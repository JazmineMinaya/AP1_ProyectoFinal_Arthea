using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class AdministradoresService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Administradores, int>
{
    public async Task<Result> Guardar(Administradores administrador)
    {
        if (!await Existe(administrador.AdministradorId))
        {
            var inserto = await Insertar(administrador);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(administrador);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int administradorId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Administradores
            .AnyAsync(e => e.AdministradorId == administradorId);
    }

    private async Task<bool> Insertar(Administradores administrador)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Administradores.Add(administrador);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Administradores administrador)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Administradores.FindAsync(administrador.AdministradorId);

        if (existente is null)
            return false;

        existente.FotoPerfil = administrador.FotoPerfil;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Administradores?>> Buscar(int administradorId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Administradores
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(e => e.AdministradorId == administradorId);
    }

    public async Task<Result> Eliminar(int administradorId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Administradores
            .Where(e => e.AdministradorId == administradorId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Administradores>>> GetList(Expression<Func<Administradores, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Administradores
            .Include(a => a.Usuario)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
