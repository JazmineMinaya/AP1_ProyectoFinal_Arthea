using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class ClientesService(
    IDbContextFactory<Contexto> contextFactory
) : IServiceResult<Clientes, int>
{
    public async Task<Result> Guardar(Clientes cliente)
    {
        if (!await Existe(cliente.ClienteId))
        {
            var inserto = await Insertar(cliente);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(cliente);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int clienteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Clientes
            .AnyAsync(c => c.ClienteId == clienteId);
    }

    private async Task<bool> Insertar(Clientes cliente)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Clientes.Add(cliente);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Clientes cliente)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var existente = await contexto.Clientes.FindAsync(cliente.ClienteId);

        if (existente is null)
            return false;

        existente.Telefono = cliente.Telefono;
        existente.Direccion = cliente.Direccion;
        existente.FotoPerfil = cliente.FotoPerfil;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Clientes?>> Buscar(int clienteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }

    public async Task<Result> Eliminar(int clienteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Clientes
            .Where(c => c.ClienteId == clienteId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }

    public async Task<Result<List<Clientes>>> GetList(Expression<Func<Clientes, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Clientes
            .Include(c => c.Usuario)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
