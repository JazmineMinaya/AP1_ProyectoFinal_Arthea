using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class ServiciosService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Servicios, int>
{
    public async Task<Result> Guardar(Servicios servicio)
    {
        if (!await Existe(servicio.ServicioId))
        {
            var inserto = await Insertar(servicio);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(servicio);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int servicioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Servicios
            .AnyAsync(s => s.ServicioId == servicioId);
    }

    private async Task<bool> Insertar(Servicios servicio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Servicios.Add(servicio);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Servicios servicio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Servicios.Update(servicio);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Servicios?>> Buscar(int servicioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Servicios
            .Include(s => s.Artista)
                .ThenInclude(a => a!.Usuario)
            .FirstOrDefaultAsync(s => s.ServicioId == servicioId);
    }

    public async Task<Result> Eliminar(int servicioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Servicios
            .Where(s => s.ServicioId == servicioId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Servicios>>> GetList(Expression<Func<Servicios, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Servicios
            .Include(s => s.Artista)
                .ThenInclude(a => a!.Usuario)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
