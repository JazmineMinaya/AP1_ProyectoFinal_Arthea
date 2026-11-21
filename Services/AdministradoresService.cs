using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace AP1_ProyectoFinal_Arthea.Services;

public class AdministradoresService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Administradores, int>
{
    private readonly PasswordHasher<Administradores> passwordHasher = new();

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

    public async Task<bool> CorreoExiste(string correo, int administradorId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Administradores
            .AnyAsync(a => a.Correo == correo && a.AdministradorId != administradorId);
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
        contexto.Update(administrador);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Administradores?>> Buscar(int administradorId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Administradores
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
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public string GenerarHash(Administradores administrador, string password)
    {
        return passwordHasher.HashPassword(administrador, password);
    }

    public bool VerificarPassword(Administradores administrador, string password)
    {
        var resultado = passwordHasher.VerifyHashedPassword(
            administrador,
            administrador.PasswordHash,
            password
        );

        return resultado != PasswordVerificationResult.Failed;
    }
}
