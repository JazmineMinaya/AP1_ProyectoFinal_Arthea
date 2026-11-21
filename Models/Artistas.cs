using System.ComponentModel.DataAnnotations;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Artistas
{
    [Key]
    public int ArtistaId { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    public Usuarios? Usuario { get; set; }

    [Required(ErrorMessage = "Este campo es requerido")]
    [Phone(ErrorMessage = "Ingrese un teléfono válido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione su disciplina artística")]
    public string Disciplina { get; set; } = string.Empty;

    public string? Biografia { get; set; }
}
