using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class ObrasService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Obras, int>
{
    public async Task<Result> Guardar(Obras obra)
    {
        if (!await Existe(obra.ObraId))
        {
            var inserto = await Insertar(obra);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(obra);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int obraId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Obras
            .AnyAsync(o => o.ObraId == obraId);
    }

    private async Task<bool> Insertar(Obras obra)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Obras.Add(obra);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Obras obra)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Obras.FindAsync(obra.ObraId);

        if (existente is null)
            return false;

        existente.ArtistaId = obra.ArtistaId;
        existente.Titulo = obra.Titulo;
        existente.Categoria = obra.Categoria;
        existente.Descripcion = obra.Descripcion;
        existente.Precio = obra.Precio;
        existente.Estado = obra.Estado;
        existente.ImagenUrl = obra.ImagenUrl;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Obras?>> Buscar(int obraId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Obras
            .Include(o => o.Artista)
            .ThenInclude(a => a!.Usuario)
            .FirstOrDefaultAsync(o => o.ObraId == obraId);
    }

    public async Task<Result> Eliminar(int obraId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Obras
            .Where(o => o.ObraId == obraId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Obras>>> GetList(Expression<Func<Obras, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Obras
            .Include(o => o.Artista)
            .ThenInclude(a => a!.Usuario)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}