using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AP1_ProyectoFinal_Arthea.Services;

public class CarritoService(
    IDbContextFactory<Contexto> contextFactory,
    AuthenticationStateProvider authenticationStateProvider
) : IServiceResult<PedidosDetalle, int>
{
    public event Action? OnChange;

    private async Task<string?> ObtenerUsuarioId()
    {
        var autenticacion = await authenticationStateProvider.GetAuthenticationStateAsync();
        var usuario = autenticacion.User;

        if (usuario.Identity?.IsAuthenticated != true || (!usuario.IsInRole("Cliente") && !usuario.IsInRole("Administrador")))
            return null;

        return usuario.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<Result> Guardar(PedidosDetalle detalle)
    {
        var guardado = !await Existe(detalle.DetalleId)
            ? await Insertar(detalle)
            : await Modificar(detalle);

        return Result.Success(guardado);
    }

    private async Task<bool> Existe(int detalleId)
    {
        var usuarioId = await ObtenerUsuarioId();

        if (usuarioId == null || detalleId <= 0)
            return false;

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.PedidosDetalles
            .AnyAsync(d => d.DetalleId == detalleId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId);
    }

    private async Task<bool> Insertar(PedidosDetalle detalle)
    {
        var usuarioId = await ObtenerUsuarioId();

        if (usuarioId == null || detalle.DetalleId != 0 || detalle.Cantidad <= 0)
            return false;

        if ((detalle.ServicioId.HasValue && detalle.ObraId.HasValue) || (!detalle.ServicioId.HasValue && !detalle.ObraId.HasValue))
            return false;

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .FirstOrDefaultAsync(p => p.PedidoId == detalle.PedidoId && p.Estado == "Carrito" && p.Cliente!.UsuarioId == usuarioId);

        if (pedido == null)
            return false;

        if (detalle.ObraId.HasValue)
        {
            var obra = await contexto.Obras
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ObraId == detalle.ObraId && o.Estado == "Disponible");

            if (obra == null || obra.Cantidad < detalle.Cantidad)
                return false;

            var cantidadEnCarrito = await contexto.PedidosDetalles
                .Where(d => d.PedidoId == pedido.PedidoId && d.ObraId == obra.ObraId)
                .SumAsync(d => d.Cantidad);

            if (cantidadEnCarrito + detalle.Cantidad > obra.Cantidad)
                return false;

            contexto.PedidosDetalles.Add(new PedidosDetalle
            {
                PedidoId = pedido.PedidoId,
                ObraId = obra.ObraId,
                ServicioId = null,
                Cantidad = detalle.Cantidad,
                Precio = Convert.ToDouble(obra.Precio),
                EstadoAprobacion = "Aprobado"
            });

            pedido.Total += detalle.Cantidad * Convert.ToDouble(obra.Precio);
        }
        else
        {
            var servicio = await contexto.Servicios
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServicioId == detalle.ServicioId && s.Estado);

            if (servicio == null || !ValidarDetalle(detalle, servicio))
                return false;

            contexto.PedidosDetalles.Add(new PedidosDetalle
            {
                PedidoId = pedido.PedidoId,
                ServicioId = servicio.ServicioId,
                ObraId = null,
                Cantidad = detalle.Cantidad,
                Precio = Convert.ToDouble(servicio.Precio),
                Requerimientos = detalle.Requerimientos,
                GuiaRequerimientos = servicio.GuiaRequerimientos,
                EnlaceReferencia = detalle.EnlaceReferencia,
                ModalidadEntrega = servicio.ModalidadEntrega,
                DireccionEntrega = detalle.DireccionEntrega,
                MedioEntrega = detalle.MedioEntrega,
                ContactoEntrega = detalle.ContactoEntrega,
                EstadoAprobacion = "Pendiente",
                Referencias = detalle.Referencias.Select(r => new PedidosReferencias
                {
                    Imagen = r.Imagen,
                    Detalle = r.Detalle
                }).ToList()
            });

            pedido.Total += detalle.Cantidad * Convert.ToDouble(servicio.Precio);
        }

        var inserto = await contexto.SaveChangesAsync() > 0;

        if (inserto)
            Notificar();

        return inserto;
    }

    private async Task<bool> Modificar(PedidosDetalle detalle)
    {
        var usuarioId = await ObtenerUsuarioId();

        if (usuarioId == null || detalle.Cantidad <= 0)
            return false;

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.PedidosDetalles
            .Include(d => d.Referencias)
            .Include(d => d.Pedido)
            .FirstOrDefaultAsync(d => d.DetalleId == detalle.DetalleId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId);

        if (existente == null || existente.Pedido == null)
            return false;

        var subtotalAnterior = existente.Subtotal;

        if (existente.ObraId.HasValue)
        {
            var obra = await contexto.Obras
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ObraId == existente.ObraId && o.Estado == "Disponible");

            if (obra == null)
                return false;

            var cantidadOtrosDetalles = await contexto.PedidosDetalles
                .Where(d => d.PedidoId == existente.PedidoId && d.ObraId == obra.ObraId && d.DetalleId != existente.DetalleId)
                .SumAsync(d => d.Cantidad);

            if (cantidadOtrosDetalles + detalle.Cantidad > obra.Cantidad)
                return false;

            existente.Cantidad = detalle.Cantidad;
            existente.Precio = Convert.ToDouble(obra.Precio);
        }
        else
        {
            var servicio = await contexto.Servicios
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServicioId == existente.ServicioId && s.Estado);

            if (servicio == null || !ValidarDetalle(detalle, servicio))
                return false;

            existente.Cantidad = detalle.Cantidad;
            existente.Precio = Convert.ToDouble(servicio.Precio);
            existente.Requerimientos = detalle.Requerimientos;
            existente.GuiaRequerimientos = servicio.GuiaRequerimientos;
            existente.EnlaceReferencia = detalle.EnlaceReferencia;
            existente.ModalidadEntrega = servicio.ModalidadEntrega;
            existente.DireccionEntrega = detalle.DireccionEntrega;
            existente.MedioEntrega = detalle.MedioEntrega;
            existente.ContactoEntrega = detalle.ContactoEntrega;

            contexto.PedidosReferencias.RemoveRange(existente.Referencias);

            existente.Referencias = detalle.Referencias.Select(r => new PedidosReferencias
            {
                Imagen = r.Imagen,
                Detalle = r.Detalle
            }).ToList();
        }

        existente.Pedido.Total += existente.Subtotal - subtotalAnterior;

        var modifico = await contexto.SaveChangesAsync() > 0;

        if (modifico)
            Notificar();

        return modifico;
    }

    public async Task<Result<PedidosDetalle?>> Buscar(int detalleId)
    {
        var usuarioId = await ObtenerUsuarioId();

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.PedidosDetalles
            .Include(d => d.Servicio)
            .Include(d => d.Obra)
            .Include(d => d.Referencias)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => usuarioId != null && d.DetalleId == detalleId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId);
    }

    public async Task<Result> Eliminar(int detalleId)
    {
        var usuarioId = await ObtenerUsuarioId();

        if (usuarioId == null)
            return Result.Success(false);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detalle = await contexto.PedidosDetalles
            .Include(d => d.Referencias)
            .Include(d => d.Pedido)
            .FirstOrDefaultAsync(d => d.DetalleId == detalleId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId);

        if (detalle == null || detalle.Pedido == null)
            return Result.Success(false);

        detalle.Pedido.Total = Math.Max(0, detalle.Pedido.Total - detalle.Subtotal);
        contexto.PedidosDetalles.Remove(detalle);

        var eliminado = await contexto.SaveChangesAsync() > 0;

        if (eliminado)
            Notificar();

        return Result.Success(eliminado);
    }

    public async Task<Result<List<PedidosDetalle>>> GetList(Expression<Func<PedidosDetalle, bool>> criterio)
    {
        var usuarioId = await ObtenerUsuarioId();

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.PedidosDetalles
            .Include(d => d.Servicio)
                .ThenInclude(s => s!.Artista)
                    .ThenInclude(a => a!.Usuario)
            .Include(d => d.Obra)
                .ThenInclude(o => o!.Artista)
                    .ThenInclude(a => a!.Usuario)
            .Include(d => d.Referencias)
            .Where(d => usuarioId != null && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Limpiar(int pedidoId)
    {
        var usuarioId = await ObtenerUsuarioId();

        if (usuarioId == null)
            return Result.Success(false);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(p => p.PedidosDetalle)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId && p.Estado == "Carrito" && p.Cliente!.UsuarioId == usuarioId);

        if (pedido == null)
            return Result.Success(false);

        contexto.PedidosDetalles.RemoveRange(pedido.PedidosDetalle);
        pedido.Total = 0;

        var limpio = await contexto.SaveChangesAsync() > 0;

        if (limpio)
            Notificar();

        return Result.Success(limpio);
    }

    public async Task<int> Cantidad(int pedidoId)
    {
        var usuarioId = await ObtenerUsuarioId();

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.PedidosDetalles
            .Where(d => usuarioId != null && d.PedidoId == pedidoId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId)
            .SumAsync(d => d.Cantidad);
    }

    public async Task<double> Total(int pedidoId)
    {
        var usuarioId = await ObtenerUsuarioId();

        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.PedidosDetalles
            .Where(d => usuarioId != null && d.PedidoId == pedidoId && d.Pedido!.Estado == "Carrito" && d.Pedido.Cliente!.UsuarioId == usuarioId)
            .SumAsync(d => d.Cantidad * d.Precio);
    }

    private bool ValidarDetalle(PedidosDetalle detalle, Servicios servicio)
    {
        if (detalle.Cantidad <= 0 || string.IsNullOrWhiteSpace(detalle.Requerimientos))
            return false;

        if (servicio.ModalidadEntrega == "Presencial" && string.IsNullOrWhiteSpace(detalle.DireccionEntrega))
            return false;

        if (servicio.ModalidadEntrega == "Digital" && (string.IsNullOrWhiteSpace(detalle.MedioEntrega) || string.IsNullOrWhiteSpace(detalle.ContactoEntrega)))
            return false;

        if (servicio.ModalidadEntrega != "Presencial" && servicio.ModalidadEntrega != "Digital")
            return false;

        if (detalle.Referencias == null || detalle.Referencias.Count > 3 || detalle.Referencias.Any(r => string.IsNullOrWhiteSpace(r.Imagen)))
            return false;

        return true;
    }

    private void Notificar()
    {
        OnChange?.Invoke();
    }
}
