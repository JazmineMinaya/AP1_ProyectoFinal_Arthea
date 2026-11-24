using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Servicios
{
    [Key]
    public int ServicioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista")]
    public int ArtistaId { get; set; }

    public Artistas? Artista { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre del servicio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una categoría")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la descripción del servicio")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un precio válido")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "Seleccione una imagen de portada para el servicio")]
    public string Imagen1 { get; set; } = string.Empty;

    public string? Imagen2 { get; set; }
    public string? Imagen3 { get; set; }
    public string? Imagen4 { get; set; }
    public string? Imagen5 { get; set; }

    public bool Estado { get; set; } = true;
}
