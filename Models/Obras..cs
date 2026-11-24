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

    [Required(ErrorMessage = "Ingrese el título de la obra")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una categoría")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la técnica utilizada")]
    public string Tecnica { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la descripción de la obra")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un precio válido")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Ingrese una cantidad válida")]
    public int Cantidad { get; set; } = 1;

    public DateTime FechaPublicacion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Seleccione un estado")]
    public string Estado { get; set; } = "Disponible"; // Disponible, Reservada, Vendida

    public string? ImagenUrl { get; set; }
}