using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Portafolios
{
    [Key]
    public int PortafolioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista")]
    public int ArtistaId { get; set; }

    [ForeignKey("ArtistaId")]
    public Artistas? Artista { get; set; }

    [Required(ErrorMessage = "Ingrese el título del portafolio")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la descripción del portafolio")]
    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Seleccione una imagen de portada")]
    public string ImagenPortada { get; set; } = string.Empty;

    public virtual ICollection<PortafoliosDetalle> PortafoliosDetalle { get; set; } = new List<PortafoliosDetalle>();
}