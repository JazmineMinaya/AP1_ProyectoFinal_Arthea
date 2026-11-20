using System.ComponentModel.DataAnnotations;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Administradores
{
    [Key]
    public int AdministradorId { get; set; }

    [Required(ErrorMessage = "Este campo es requerido")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    [Phone(ErrorMessage = "Ingrese un teléfono válido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    public string PasswordHash { get; set; } = string.Empty;

    public string Rol { get; set; } = "Administrador";

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public bool Activo { get; set; } = true;

    public string? FotoPerfil { get; set; }
}
