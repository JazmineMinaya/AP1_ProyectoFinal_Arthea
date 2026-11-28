using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using Aplicada1.Core;

namespace AP1_ProyectoFinal_Arthea.Services;

public class PedidosService(
    IDbContextFactory<Contexto> contextFactory,
    UserManager<Usuarios> userManager
) : IServiceResult<Pedidos, int>
{
    public async Task<Result> Guardar(Pedidos pedido)
    {
        if (!await Existe(pedido.PedidoId))
            return Result.Success(await Insertar(pedido));

        return Result.Success(await Modificar(pedido));
    }

    private async Task<bool> Existe(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Pedidos.AnyAsync(e => e.PedidoId == pedidoId);
    }

    public async Task<Result<Pedidos?>> ObtenerOCrearCarrito(string usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId);

        if (usuario == null || !usuario.Estado)
            return Result.Success<Pedidos?>(null);

        var esCliente = await userManager.IsInRoleAsync(usuario, "Cliente");
        var esAdministrador = await userManager.IsInRoleAsync(usuario, "Administrador");

        if (!esCliente && !esAdministrador)
            return Result.Success<Pedidos?>(null);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var cliente = await contexto.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        var carrito = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.Estado == "Carrito");

        if (carrito == null && cliente != null)
        {
            carrito = await contexto.Pedidos
                .Include(p => p.PedidosDetalle)
                .FirstOrDefaultAsync(p => p.ClienteId == cliente.ClienteId && p.UsuarioId == null && p.Estado == "Carrito");

            if (carrito != null)
            {
                carrito.UsuarioId = usuarioId;
                await contexto.SaveChangesAsync();
            }
        }

        if (carrito != null)
            return Result.Success<Pedidos?>(carrito);

        carrito = new Pedidos
        {
            UsuarioId = usuarioId,
            ClienteId = cliente?.ClienteId,
            Fecha = DateTime.Now,
            Estado = "Carrito",
            EstadoPago = "Pendiente",
            Total = 0
        };

        contexto.Pedidos.Add(carrito);
        await contexto.SaveChangesAsync();

        return Result.Success<Pedidos?>(carrito);
    }

    public async Task<Result> ConfirmarCarrito(int pedidoId, string usuarioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Referencias)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId && p.UsuarioId == usuarioId && p.Estado == "Carrito");

        if (pedido == null || !await ValidarDetalles(contexto, pedido))
            return Result.Success(false);

        var ahora = DateTime.UtcNow;

        pedido.Estado = "PendienteAprobacion";
        pedido.EstadoPago = "Pendiente";
        pedido.FechaConfirmacion = ahora;
        pedido.FechaLimiteAprobacion = ahora.AddHours(24);

        foreach (var detalle in pedido.PedidosDetalle)
        {
            detalle.EstadoAprobacion = "Pendiente";
            detalle.FechaRespuesta = null;
            detalle.MotivoRechazo = null;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    private async Task<bool> ValidarDetalles(Contexto contexto, Pedidos pedido)
    {
        if (pedido.PedidosDetalle == null || !pedido.PedidosDetalle.Any())
            return false;

        if (pedido.ClienteId.HasValue)
        {
            var cliente = await contexto.Clientes.FirstOrDefaultAsync(c => c.ClienteId == pedido.ClienteId.Value);

            if (cliente == null)
                return false;

            if (string.IsNullOrWhiteSpace(pedido.UsuarioId))
                pedido.UsuarioId = cliente.UsuarioId;
            else if (pedido.UsuarioId != cliente.UsuarioId)
                return false;
        }

        if (string.IsNullOrWhiteSpace(pedido.UsuarioId))
            return false;

        var usuario = await contexto.Users.FirstOrDefaultAsync(u => u.Id == pedido.UsuarioId && u.Estado);

        if (usuario == null)
            return false;

        var esCliente = await userManager.IsInRoleAsync(usuario, "Cliente");
        var esAdministrador = await userManager.IsInRoleAsync(usuario, "Administrador");

        if (!esCliente && !esAdministrador)
            return false;

        foreach (var detalle in pedido.PedidosDetalle)
        {
            if (detalle.Cantidad <= 0 || detalle.ServicioId.HasValue == detalle.ObraId.HasValue)
                return false;

            if (detalle.ObraId.HasValue)
            {
                var obra = await contexto.Obras.AsNoTracking()
                    .FirstOrDefaultAsync(o => o.ObraId == detalle.ObraId && o.Estado == "Disponible");

                if (obra == null)
                    return false;

                var cantidadSolicitada = pedido.PedidosDetalle
                    .Where(d => d.ObraId == obra.ObraId)
                    .Sum(d => d.Cantidad);

                if (cantidadSolicitada > obra.Cantidad)
                    return false;

                detalle.Precio = Convert.ToDouble(obra.Precio);
            }
            else
            {
                var servicio = await contexto.Servicios.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ServicioId == detalle.ServicioId);

                if (servicio == null || !servicio.Estado)
                    return false;

                if (string.IsNullOrWhiteSpace(detalle.Requerimientos))
                    return false;

                if (servicio.ModalidadEntrega != "Digital" && servicio.ModalidadEntrega != "Presencial")
                    return false;

                if (servicio.ModalidadEntrega == "Presencial" && string.IsNullOrWhiteSpace(detalle.DireccionEntrega))
                    return false;

                if (servicio.ModalidadEntrega == "Digital" && (string.IsNullOrWhiteSpace(detalle.MedioEntrega) || string.IsNullOrWhiteSpace(detalle.ContactoEntrega)))
                    return false;

                if (detalle.Referencias == null || detalle.Referencias.Count > 3 || detalle.Referencias.Any(r => string.IsNullOrWhiteSpace(r.Imagen)))
                    return false;

                detalle.Precio = Convert.ToDouble(servicio.Precio);
                detalle.ModalidadEntrega = servicio.ModalidadEntrega;
                detalle.GuiaRequerimientos = servicio.GuiaRequerimientos;
            }
        }

        pedido.Total = pedido.PedidosDetalle.Sum(d => d.Subtotal);
        return true;
    }

    private async Task<bool> Insertar(Pedidos pedido)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        if (!await ValidarDetalles(contexto, pedido))
            return false;

        pedido.Estado = "Pendiente";
        pedido.EstadoPago = "Pendiente";
        pedido.FechaLimiteAprobacion = default;
        pedido.FechaConfirmacion = null;
        pedido.FechaPago = null;
        pedido.Factura = null;
        pedido.Cliente = null;
        pedido.Usuario = null;

        foreach (var detalle in pedido.PedidosDetalle)
        {
            detalle.EstadoAprobacion = "Pendiente";
            detalle.FechaRespuesta = null;
            detalle.MotivoRechazo = null;
            detalle.Servicio = null;
            detalle.Obra = null;
        }

        contexto.Pedidos.Add(pedido);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Pedidos pedido)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedidoInicial = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Referencias)
            .FirstOrDefaultAsync(p => p.PedidoId == pedido.PedidoId);

        if (pedidoInicial == null || pedidoInicial.Estado != "Pendiente" || pedidoInicial.EstadoPago != "Pendiente" || pedidoInicial.FechaConfirmacion != null)
            return false;

        if (!await ValidarDetalles(contexto, pedido))
            return false;

        contexto.RemoveRange(pedidoInicial.PedidosDetalle);

        pedidoInicial.Fecha = pedido.Fecha;
        pedidoInicial.ClienteId = pedido.ClienteId;
        pedidoInicial.UsuarioId = pedido.UsuarioId;
        pedidoInicial.Total = pedido.Total;

        pedidoInicial.PedidosDetalle = pedido.PedidosDetalle.Select(d => new PedidosDetalle
        {
            PedidoId = pedidoInicial.PedidoId,
            ServicioId = d.ServicioId,
            ObraId = d.ObraId,
            Cantidad = d.Cantidad,
            Precio = d.Precio,
            Requerimientos = d.Requerimientos,
            GuiaRequerimientos = d.GuiaRequerimientos,
            EnlaceReferencia = d.EnlaceReferencia,
            ModalidadEntrega = d.ModalidadEntrega,
            DireccionEntrega = d.DireccionEntrega,
            MedioEntrega = d.MedioEntrega,
            ContactoEntrega = d.ContactoEntrega,
            EstadoAprobacion = "Pendiente",
            Referencias = d.Referencias.Select(r => new PedidosReferencias
            {
                Imagen = r.Imagen
            }).ToList()
        }).ToList();

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Pedidos?>> Buscar(int pedidoId)
    {
        await FinalizarAprobaciones(pedidoId);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Cliente)
                .ThenInclude(c => c!.Usuario)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Referencias)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Servicio)
                    .ThenInclude(s => s!.Artista)
                        .ThenInclude(a => a!.Usuario)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Obra)
                    .ThenInclude(o => o!.Artista)
                        .ThenInclude(a => a!.Usuario)
            .Include(p => p.Factura)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
    }

    public async Task<Result> Eliminar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

        if (pedido == null || pedido.Estado != "Pendiente" || pedido.EstadoPago != "Pendiente" || pedido.FechaConfirmacion != null)
            return Result.Success(false);

        contexto.RemoveRange(pedido.PedidosDetalle);
        contexto.Pedidos.Remove(pedido);

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<List<Pedidos>>> GetList(Expression<Func<Pedidos, bool>> criterio)
    {
        await ActualizarVencidos();

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Cliente)
                .ThenInclude(c => c!.Usuario)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Referencias)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Servicio)
                    .ThenInclude(s => s!.Artista)
                        .ThenInclude(a => a!.Usuario)
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Obra)
                    .ThenInclude(o => o!.Artista)
                        .ThenInclude(a => a!.Usuario)
            .Include(p => p.Factura)
            .Where(p => p.Estado != "Carrito")
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> EnviarAprobacion(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
                .ThenInclude(d => d.Referencias)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

        if (pedido == null || pedido.Estado != "Pendiente" || pedido.EstadoPago != "Pendiente" || pedido.FechaConfirmacion != null)
            return Result.Success(false);

        if (!await ValidarDetalles(contexto, pedido))
            return Result.Success(false);

        var ahora = DateTime.UtcNow;

        pedido.FechaConfirmacion = ahora;
        pedido.FechaLimiteAprobacion = ahora.AddHours(24);
        pedido.Estado = "PendienteAprobacion";

        foreach (var detalle in pedido.PedidosDetalle)
        {
            detalle.EstadoAprobacion = "Pendiente";
            detalle.FechaRespuesta = null;
            detalle.MotivoRechazo = null;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> AprobarDetalle(int detalleId, string usuarioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detalle = await contexto.PedidosDetalles
            .Include(d => d.Pedido)
                .ThenInclude(p => p!.PedidosDetalle)
            .FirstOrDefaultAsync(d => d.DetalleId == detalleId);

        if (detalle == null || detalle.Pedido?.Estado != "PendienteAprobacion" || detalle.EstadoAprobacion != "Pendiente")
            return Result.Success(false);

        if (!await PuedeResponderDetalle(contexto, detalle, usuarioId))
            return Result.Success(false);

        if (DateTime.UtcNow >= detalle.Pedido.FechaLimiteAprobacion)
        {
            await FinalizarAprobaciones(detalle.PedidoId);
            return Result.Success(false);
        }

        detalle.EstadoAprobacion = "Aprobado";
        detalle.FechaRespuesta = DateTime.UtcNow;
        detalle.MotivoRechazo = null;

        var aprobado = await contexto.SaveChangesAsync() > 0;

        if (aprobado)
            await FinalizarAprobaciones(detalle.PedidoId);

        return Result.Success(aprobado);
    }

    public async Task<Result> RechazarDetalle(int detalleId, string usuarioId, string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            return Result.Success(false);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detalle = await contexto.PedidosDetalles
            .Include(d => d.Pedido)
                .ThenInclude(p => p!.PedidosDetalle)
            .FirstOrDefaultAsync(d => d.DetalleId == detalleId);

        if (detalle == null || detalle.Pedido?.Estado != "PendienteAprobacion" || detalle.EstadoAprobacion != "Pendiente")
            return Result.Success(false);

        if (!await PuedeResponderDetalle(contexto, detalle, usuarioId))
            return Result.Success(false);

        if (DateTime.UtcNow >= detalle.Pedido.FechaLimiteAprobacion)
        {
            await FinalizarAprobaciones(detalle.PedidoId);
            return Result.Success(false);
        }

        detalle.EstadoAprobacion = "Rechazado";
        detalle.MotivoRechazo = motivo.Trim();
        detalle.FechaRespuesta = DateTime.UtcNow;

        var rechazado = await contexto.SaveChangesAsync() > 0;

        if (rechazado)
            await FinalizarAprobaciones(detalle.PedidoId);

        return Result.Success(rechazado);
    }

    public async Task<Result> FinalizarAprobaciones(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

        if (pedido == null || pedido.Estado != "PendienteAprobacion")
            return Result.Success(false);

        var ahora = DateTime.UtcNow;
        bool vencido = ahora >= pedido.FechaLimiteAprobacion;
        bool todosRespondieron = pedido.PedidosDetalle.All(d => d.EstadoAprobacion != "Pendiente");

        if (!vencido && !todosRespondieron)
            return Result.Success(false);

        if (vencido)
        {
            foreach (var detalle in pedido.PedidosDetalle.Where(d => d.EstadoAprobacion == "Pendiente"))
            {
                detalle.EstadoAprobacion = "Vencido";
                detalle.FechaRespuesta = ahora;
            }
        }

        pedido.Total = pedido.PedidosDetalle
            .Where(d => d.EstadoAprobacion == "Aprobado")
            .Sum(d => d.Subtotal);

        if (pedido.Total > 0)
        {
            pedido.Estado = "PendientePago";
        }
        else
        {
            pedido.Estado = "Cancelado";
            pedido.FechaCancelacion = ahora;
            pedido.MotivoCancelacion = "Ninguna obra o servicio quedó aprobado";
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> ActualizarVencidos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedidosIds = await contexto.Pedidos
            .Where(p => p.Estado == "PendienteAprobacion" && p.FechaLimiteAprobacion <= DateTime.UtcNow)
            .Select(p => p.PedidoId)
            .ToListAsync();

        bool actualizado = false;

        foreach (var pedidoId in pedidosIds)
        {
            await FinalizarAprobaciones(pedidoId);
            actualizado = true;
        }

        return Result.Success(actualizado);
    }

    public async Task<Result> ConfirmarPago(int pedidoId, string usuarioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var autorizado = await contexto.Pedidos.AnyAsync(p => p.PedidoId == pedidoId && p.UsuarioId == usuarioId);

        if (!autorizado)
            return Result.Success(false);

        return await ConfirmarPago(pedidoId);
    }

    public async Task<Result> ConfirmarPago(int pedidoId)
    {
        await FinalizarAprobaciones(pedidoId);

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
            .Include(p => p.Factura)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

        if (pedido == null || pedido.Estado != "PendientePago" || pedido.EstadoPago != "Pendiente" || pedido.Factura != null)
            return Result.Success(false);

        var detallesAprobados = pedido.PedidosDetalle
            .Where(d => d.EstadoAprobacion == "Aprobado")
            .ToList();

        var total = detallesAprobados.Sum(d => d.Subtotal);

        if (total <= 0 || Math.Abs(total - pedido.Total) > 0.01)
            return Result.Success(false);

        var obrasSolicitadas = detallesAprobados
            .Where(d => d.ObraId.HasValue)
            .GroupBy(d => d.ObraId!.Value)
            .Select(g => new { ObraId = g.Key, Cantidad = g.Sum(d => d.Cantidad) })
            .ToList();

        foreach (var item in obrasSolicitadas)
        {
            var obra = await contexto.Obras.FirstOrDefaultAsync(o => o.ObraId == item.ObraId);

            if (obra == null || obra.Estado != "Disponible" || obra.Cantidad < item.Cantidad)
                return Result.Success(false);

            obra.Cantidad -= item.Cantidad;

            if (obra.Cantidad == 0)
                obra.Estado = "Vendida";
        }

        pedido.Estado = "Pagado";
        pedido.EstadoPago = "Pagado";
        pedido.FechaPago = DateTime.UtcNow;

        pedido.Factura = new Facturas
        {
            Fecha = DateTime.UtcNow,
            NumeroFactura = $"ART-{pedido.PedidoId:D6}-{Guid.NewGuid():N}",
            Total = total,
            Estado = "Pagada"
        };

        var pagado = await contexto.SaveChangesAsync() > 0;

        if (pagado)
            await transaccion.CommitAsync();

        return Result.Success(pagado);
    }

    public async Task<Result> Preparar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Pagado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.Estado = "EnProceso";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Enviar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "EnProceso" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.Estado = "Enviado";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Recibir(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Enviado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.Estado = "Entregado";
        pedido.FechaRecepcion = DateTime.UtcNow;

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Completar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Entregado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.Estado = "Completado";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Cancelar(int pedidoId, string motivo)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.EstadoPago != "Pendiente" || (pedido.Estado != "Pendiente" && pedido.Estado != "PendienteAprobacion" && pedido.Estado != "PendientePago"))
            return Result.Success(false);

        if (string.IsNullOrWhiteSpace(motivo))
            return Result.Success(false);

        pedido.Estado = "Cancelado";
        pedido.FechaCancelacion = DateTime.UtcNow;
        pedido.MotivoCancelacion = motivo.Trim();

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> ConfirmarReembolso(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Cancelado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.MontoReembolsado = pedido.Total;
        pedido.FechaReembolso = DateTime.UtcNow;
        pedido.EstadoPago = "Reembolsado";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    private async Task<bool> PuedeResponderDetalle(Contexto contexto, PedidosDetalle detalle, string usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId);

        if (usuario == null || !usuario.Estado)
            return false;

        if (await userManager.IsInRoleAsync(usuario, "Administrador"))
            return true;

        if (!await userManager.IsInRoleAsync(usuario, "Artista"))
            return false;

        if (detalle.ObraId.HasValue)
            return await contexto.Obras.AnyAsync(o => o.ObraId == detalle.ObraId && o.Artista!.UsuarioId == usuarioId);

        if (detalle.ServicioId.HasValue)
            return await contexto.Servicios.AnyAsync(s => s.ServicioId == detalle.ServicioId && s.Artista!.UsuarioId == usuarioId);

        return false;
    }

    public async Task<bool> ConfirmarPagoComprador(int pedidoId, string usuarioId)
    {
        if (pedidoId <= 0 || string.IsNullOrWhiteSpace(usuarioId))
            return false;

        await using (var contexto = await contextFactory.CreateDbContextAsync())
        {
            var perteneceAlComprador = await contexto.Pedidos.AsNoTracking().AnyAsync(p =>
                p.PedidoId == pedidoId &&
                p.UsuarioId == usuarioId &&
                p.Estado == "PendientePago" &&
                p.EstadoPago == "Pendiente" &&
                p.Total > 0);

            if (!perteneceAlComprador)
                return false;
        }

        await ConfirmarPago(pedidoId);

        await using var verificacion = await contextFactory.CreateDbContextAsync();

        return await verificacion.Pedidos.AsNoTracking().AnyAsync(p =>
            p.PedidoId == pedidoId &&
            p.UsuarioId == usuarioId &&
            p.EstadoPago == "Pagado" &&
            p.Factura != null);
    }
}
