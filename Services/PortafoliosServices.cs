using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class PortafoliosService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Portafolios, int>
{
    public async Task<Result> Guardar(Portafolios portafolio)
    {
        if (!await Existe(portafolio.PortafolioId))
        {
            var inserto = await Insertar(portafolio);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(portafolio);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int portafolioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Portafolios
            .AnyAsync(p => p.PortafolioId == portafolioId);
    }

    private async Task<bool> Insertar(Portafolios portafolio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        // Evitar que EF intente reinsertar entidades de navegación existentes
        portafolio.Artista = null;
        foreach (var detalle in portafolio.PortafoliosDetalle)
        {
            detalle.Obra = null;
            detalle.Portafolio = null;
        }

        contexto.Portafolios.Add(portafolio);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Portafolios portafolio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Portafolios
            .Include(p => p.PortafoliosDetalle)
            .FirstOrDefaultAsync(p => p.PortafolioId == portafolio.PortafolioId);

        if (existente is null)
            return false;

        existente.ArtistaId = portafolio.ArtistaId;
        existente.Titulo = portafolio.Titulo;
        existente.Descripcion = portafolio.Descripcion;
        existente.Estado = portafolio.Estado;
        existente.ImagenPortada = portafolio.ImagenPortada;

        // Reemplazar detalles anteriores por los actuales
        contexto.PortafoliosDetalle.RemoveRange(existente.PortafoliosDetalle);

        foreach (var item in portafolio.PortafoliosDetalle)
        {
            existente.PortafoliosDetalle.Add(new PortafoliosDetalle
            {
                PortafolioId = existente.PortafolioId,
                ObraId = item.ObraId,
                NotaCuratorial = item.NotaCuratorial
            });
        }

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Portafolios?>> Buscar(int portafolioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Portafolios
            .Include(p => p.Artista)
                .ThenInclude(a => a!.Usuario)
            .Include(p => p.PortafoliosDetalle)
                .ThenInclude(d => d.Obra)
            .FirstOrDefaultAsync(p => p.PortafolioId == portafolioId);
    }

    public async Task<Result> Eliminar(int portafolioId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        await contexto.PortafoliosDetalle
            .Where(d => d.PortafolioId == portafolioId)
            .ExecuteDeleteAsync();

        var eliminado = await contexto.Portafolios
            .Where(p => p.PortafolioId == portafolioId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Portafolios>>> GetList(Expression<Func<Portafolios, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Portafolios
            .Include(p => p.Artista)
                .ThenInclude(a => a!.Usuario)
            .Include(p => p.PortafoliosDetalle)
                .ThenInclude(d => d.Obra)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}