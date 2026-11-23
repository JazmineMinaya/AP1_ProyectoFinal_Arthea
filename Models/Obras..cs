using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Obras
{
    [Key]
    public int ObraId { get; set; }

    [Required(ErrorMessage = "Seleccione un artista")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista válido")]
    public int ArtistaId { get; set; }

    [ForeignKey("ArtistaId")]
    public Artistas? Artista { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es requerida")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "La técnica es obligatoria.")]
    public string Tecnica { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Cantidad { get; set; } = 1;

    public DateTime FechaPublicacion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Seleccione un estado")]
    public string Estado { get; set; } = "Disponible"; // Disponible, Reservada, Vendida

    public string? ImagenUrl { get; set; }
}