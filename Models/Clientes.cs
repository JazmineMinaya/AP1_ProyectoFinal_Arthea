using System.ComponentModel.DataAnnotations;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Clientes
{
    [Key]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Seleccione un usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    public Usuarios? Usuario { get; set; }

    [Required(ErrorMessage = "Ingrese su número de teléfono")]
    [Phone(ErrorMessage = "Ingrese un teléfono válido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su dirección")]
    public string Direccion { get; set; } = string.Empty;

    public string? FotoPerfil { get; set; }
}