using System.ComponentModel.DataAnnotations;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Administradores
{
    [Key]
    public int AdministradorId { get; set; }

    [Required(ErrorMessage = "Seleccione un usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    public Usuarios? Usuario { get; set; }

    public string? FotoPerfil { get; set; }
}
