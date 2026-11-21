using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class ArtistasService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Artistas, int>
{
    public async Task<Result> Guardar(Artistas artista)
    {
        if (!await Existe(artista.ArtistaId))
        {
            var inserto = await Insertar(artista);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(artista);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int artistaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Artistas
            .AnyAsync(a => a.ArtistaId == artistaId);
    }

    private async Task<bool> Insertar(Artistas artista)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Artistas.Add(artista);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Artistas artista)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Artistas.FindAsync(artista.ArtistaId);

        if (existente is null)
            return false;

        existente.Telefono = artista.Telefono;
        existente.Disciplina = artista.Disciplina;
        existente.Biografia = artista.Biografia;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Artistas?>> Buscar(int artistaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Artistas
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.ArtistaId == artistaId);
    }

    public async Task<Result> Eliminar(int artistaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Artistas
            .Where(a => a.ArtistaId == artistaId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Artistas>>> GetList(Expression<Func<Artistas, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Artistas
            .Include(a => a.Usuario)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
