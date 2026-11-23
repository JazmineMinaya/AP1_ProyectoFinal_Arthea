using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Aplicada1.Core;

namespace AP1_ProyectoFinal_Arthea.Services;

public class PedidosService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Pedidos, int>
{
    public async Task<Result> Guardar(Pedidos pedido)
    {
        if (!await Existe(pedido.PedidoId))
        {
            var inserto = await Insertar(pedido);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(pedido);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Pedidos
            .AnyAsync(e => e.PedidoId == pedidoId);
    }

    private async Task<bool> Insertar(Pedidos pedido)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Pedidos.Add(pedido);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Pedidos pedido)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedidoInicial = await contexto.Pedidos
            .Include(a => a.PedidosDetalle)
            .FirstOrDefaultAsync(a => a.PedidoId == pedido.PedidoId);

        if (pedidoInicial == null || pedidoInicial.Estado != "Pendiente" || pedidoInicial.EstadoPago != "Pendiente")
            return false;

        contexto.RemoveRange(pedidoInicial.PedidosDetalle);

        pedidoInicial.Fecha = pedido.Fecha;
        pedidoInicial.ClienteId = pedido.ClienteId;
        pedidoInicial.ArtistaId = pedido.ArtistaId;
        pedidoInicial.DireccionEntrega = pedido.DireccionEntrega;
        pedidoInicial.Total = pedido.Total;
        pedidoInicial.PedidosDetalle = pedido.PedidosDetalle
            .Select(d => new PedidosDetalle
            {
                PedidoId = pedidoInicial.PedidoId,
                ServicioId = d.ServicioId,
                Cantidad = d.Cantidad,
                Precio = d.Precio
            })
            .ToList();

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Pedidos?>> Buscar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Pedidos
            .Include(a => a.PedidosDetalle)
                .ThenInclude(d => d.Servicio)
            .FirstOrDefaultAsync(a => a.PedidoId == pedidoId);
    }

    public async Task<Result> Eliminar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos
            .Include(a => a.PedidosDetalle)
            .FirstOrDefaultAsync(a => a.PedidoId == pedidoId);

        if (pedido == null || pedido.Estado != "Pendiente" || pedido.EstadoPago != "Pendiente")
            return Result.Success(false);

        contexto.RemoveRange(pedido.PedidosDetalle);
        contexto.Pedidos.Remove(pedido);

        var eliminado = await contexto.SaveChangesAsync() > 0;
        return Result.Success(eliminado);
    }

    public async Task<Result<List<Pedidos>>> GetList(Expression<Func<Pedidos, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Pedidos
            .Include(a => a.PedidosDetalle)
                .ThenInclude(d => d.Servicio)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> ConfirmarPago(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Pendiente" || pedido.EstadoPago != "Pendiente")
            return Result.Success(false);

        pedido.Estado = "Aprobado";
        pedido.EstadoPago = "Pagado";
        pedido.FechaPago = DateTime.Now;

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Preparar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Aprobado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.Estado = "En preparación";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Enviar(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "En preparación")
            return Result.Success(false);

        pedido.Estado = "Enviado";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Recibir(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Enviado")
            return Result.Success(false);

        pedido.Estado = "Recibido";
        pedido.FechaRecepcion = DateTime.Now;

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> Cancelar(int pedidoId, string motivo, bool esArtista)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado == "Cancelado" || pedido.Estado == "Enviado" || pedido.Estado == "Recibido")
            return Result.Success(false);

        if (!esArtista && pedido.Estado != "Pendiente" && pedido.Estado != "Aprobado")
            return Result.Success(false);

        pedido.Estado = "Cancelado";
        pedido.FechaCancelacion = DateTime.Now;
        pedido.MotivoCancelacion = motivo;

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> ConfirmarReembolso(int pedidoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var pedido = await contexto.Pedidos.FindAsync(pedidoId);

        if (pedido == null || pedido.Estado != "Cancelado" || pedido.EstadoPago != "Pagado")
            return Result.Success(false);

        pedido.MontoReembolsado = pedido.Total;
        pedido.FechaReembolso = DateTime.Now;
        pedido.EstadoPago = "Reembolsado";

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }
}
