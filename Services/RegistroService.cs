using AP1_ProyectoFinal_Arthea.Context;
using AP1_ProyectoFinal_Arthea.Models;
using Aplicada1.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AP1_ProyectoFinal_Arthea.Services;

public class RegistroService(
    IDbContextFactory<Contexto> contextFactory,
    Contexto contexto,
    UserManager<Usuarios> userManager,
    RoleManager<IdentityRole> roleManager,
    IWebHostEnvironment environment,
    ILogger<RegistroService> logger
) : IServiceResult<Usuarios, string>
{
    public async Task<Result> Guardar(Usuarios usuario)
    {
        if (!await Existe(usuario.Id))
        {
            var inserto = await Insertar(usuario);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(usuario);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(string usuarioId)
    {
        return !string.IsNullOrWhiteSpace(usuarioId) && await userManager.FindByIdAsync(usuarioId) != null;
    }

    private Task<bool> Insertar(Usuarios usuario)
    {
        return Task.FromResult(false);
    }

    private async Task<bool> Modificar(Usuarios usuario)
    {
        var existente = await userManager.FindByIdAsync(usuario.Id);

        if (existente is null)
            return false;

        existente.Nombres = usuario.Nombres;
        existente.Apellidos = usuario.Apellidos;
        existente.Email = usuario.Email;
        existente.UserName = usuario.UserName;
        existente.Estado = usuario.Estado;

        var resultado = await userManager.UpdateAsync(existente);
        return resultado.Succeeded;
    }

    public async Task<Result<Usuarios?>> Buscar(string usuarioId)
    {
        return await userManager.FindByIdAsync(usuarioId);
    }

    public async Task<Result> Eliminar(string usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId);

        if (usuario is null)
            return Result.Success(false);

        var resultado = await userManager.DeleteAsync(usuario);
        return Result.Success(resultado.Succeeded);
    }

    public async Task<Result<List<Usuarios>>> GetList(Expression<Func<Usuarios, bool>> criterio)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        return await db.Users.Where(criterio).AsNoTracking().ToListAsync();
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarCliente(string nombres, string apellidos, string correo, string telefono, string direccion, string password, string? imagenBase64)
    {
        return await Registrar(nombres, apellidos, correo, telefono, direccion, null, null, password, imagenBase64, "Cliente");
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarArtista(string nombres, string apellidos, string correo, string telefono, string disciplina, string? biografia, string password, string? imagenBase64)
    {
        return await Registrar(nombres, apellidos, correo, telefono, null, disciplina, biografia, password, imagenBase64, "Artista");
    }

    private async Task<(bool Exito, string Mensaje)> Registrar(string nombres, string apellidos, string correo, string telefono, string? direccion, string? disciplina, string? biografia, string password, string? imagenBase64, string rol)
    {
        nombres = nombres.Trim();
        apellidos = apellidos.Trim();
        correo = correo.Trim();
        telefono = telefono.Trim();

        if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(password))
            return (false, "Complete todos los campos obligatorios.");

        if (!new EmailAddressAttribute().IsValid(correo))
            return (false, "Ingrese un correo electrónico válido.");

        if (rol == "Cliente" && string.IsNullOrWhiteSpace(direccion))
            return (false, "Ingrese su dirección.");

        if (rol == "Artista" && string.IsNullOrWhiteSpace(disciplina))
            return (false, "Seleccione una disciplina artística.");

        if (await userManager.FindByEmailAsync(correo) != null)
            return (false, "Este correo electrónico ya está registrado.");

        byte[]? bytesImagen = null;
        string? extensionImagen = null;

        if (!string.IsNullOrWhiteSpace(imagenBase64))
        {
            try
            {
                var partes = imagenBase64.Split(',', 2);

                if (partes.Length != 2)
                    return (false, "El formato de la fotografía no es válido.");

                extensionImagen = partes[0] switch
                {
                    "data:image/jpeg;base64" => ".jpg",
                    "data:image/png;base64" => ".png",
                    _ => null
                };

                if (extensionImagen is null)
                    return (false, "Solo se permiten fotografías JPG, JPEG y PNG.");

                bytesImagen = Convert.FromBase64String(partes[1]);

                if (bytesImagen.Length == 0 || bytesImagen.Length > 5 * 1024 * 1024)
                    return (false, "La fotografía no puede superar los 5 MB.");

                bool esJpeg = bytesImagen.Length >= 3 && bytesImagen[0] == 0xFF && bytesImagen[1] == 0xD8 && bytesImagen[2] == 0xFF;
                bool esPng = bytesImagen.Length >= 8 && bytesImagen[0] == 0x89 && bytesImagen[1] == 0x50 && bytesImagen[2] == 0x4E && bytesImagen[3] == 0x47 && bytesImagen[4] == 0x0D && bytesImagen[5] == 0x0A && bytesImagen[6] == 0x1A && bytesImagen[7] == 0x0A;

                if ((extensionImagen == ".jpg" && !esJpeg) || (extensionImagen == ".png" && !esPng))
                    return (false, "El contenido de la fotografía no es válido.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error al procesar la fotografía del registro.");
                return (false, "No se pudo procesar la fotografía.");
            }
        }

        try
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                var resultadoRol = await roleManager.CreateAsync(new IdentityRole(rol));

                if (!resultadoRol.Succeeded && !await roleManager.RoleExistsAsync(rol))
                    return (false, string.Join(" ", resultadoRol.Errors.Select(e => e.Description)));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al comprobar o crear el rol {Rol}.", rol);
            return (false, "No se pudo preparar el registro. Inténtelo nuevamente.");
        }

        string? rutaFoto = null;
        bool confirmado = false;

        try
        {
            await using (var transaccion = await contexto.Database.BeginTransactionAsync())
            {
                var usuario = new Usuarios
                {
                    UserName = correo,
                    Email = correo,
                    Nombres = nombres,
                    Apellidos = apellidos,
                    FechaRegistro = DateTime.Now,
                    Estado = true
                };

                var resultadoUsuario = await userManager.CreateAsync(usuario, password);

                if (!resultadoUsuario.Succeeded)
                    return (false, string.Join(" ", resultadoUsuario.Errors.Select(e => e.Description)));

                var resultadoAsignacion = await userManager.AddToRoleAsync(usuario, rol);

                if (!resultadoAsignacion.Succeeded)
                    throw new InvalidOperationException(string.Join(" ", resultadoAsignacion.Errors.Select(e => e.Description)));

                if (bytesImagen != null && extensionImagen != null)
                    rutaFoto = await GuardarImagen(bytesImagen, extensionImagen);

                if (rol == "Cliente")
                {
                    contexto.Clientes.Add(new Clientes
                    {
                        UsuarioId = usuario.Id,
                        Telefono = telefono,
                        Direccion = direccion!.Trim(),
                        FotoPerfil = rutaFoto
                    });
                }
                else
                {
                    contexto.Artistas.Add(new Artistas
                    {
                        UsuarioId = usuario.Id,
                        Telefono = telefono,
                        Disciplina = disciplina!.Trim(),
                        Biografia = biografia?.Trim(),
                        FotoPerfil = rutaFoto
                    });
                }

                await contexto.SaveChangesAsync();
                await transaccion.CommitAsync();
                confirmado = true;
            }

            return (true, "Su cuenta se ha registrado correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al registrar un usuario con rol {Rol}.", rol);

            if (!confirmado && !string.IsNullOrWhiteSpace(rutaFoto))
            {
                try
                {
                    var archivo = Path.Combine(environment.WebRootPath, rutaFoto.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (File.Exists(archivo))
                        File.Delete(archivo);
                }
                catch (Exception errorImagen)
                {
                    logger.LogWarning(errorImagen, "No se pudo eliminar la fotografía después de un registro fallido.");
                }
            }

            return (false, "No se pudo completar el registro. Inténtelo nuevamente.");
        }
    }

    private async Task<string> GuardarImagen(byte[] imagen, string extension)
    {
        var carpeta = Path.Combine(environment.WebRootPath, "imagenes", "perfiles");
        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid():N}{extension}";
        var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

        await File.WriteAllBytesAsync(rutaArchivo, imagen);

        return $"/imagenes/perfiles/{nombreArchivo}";
    }
}
