using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class FacturasService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Facturas, int>
{
    public async Task<Result> Guardar(Facturas factura)
    {
        if (!await Existe(factura.FacturaId))
        {
            var inserto = await Insertar(factura);
            return Result.Success(inserto);
        }
        else
        {
            return Result.Success(false);
        }
    }

    private async Task<bool> Existe(int facturaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Facturas
            .AnyAsync(f => f.FacturaId == facturaId);
    }

    private async Task<bool> Insertar(Facturas factura)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Facturas.Add(factura);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Facturas?>> Buscar(int facturaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Facturas
            .Include(f => f.Pedido)
                .ThenInclude(p => p!.PedidosDetalle)
            .FirstOrDefaultAsync(f => f.FacturaId == facturaId);
    }

    public Task<Result> Eliminar(int facturaId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Facturas>>> GetList(Expression<Func<Facturas, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Facturas
            .Include(f => f.Pedido)
                .ThenInclude(p => p!.PedidosDetalle)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
