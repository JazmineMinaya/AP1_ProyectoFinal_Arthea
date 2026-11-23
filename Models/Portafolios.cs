using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class Portafolios
{
    [Key]
    public int PortafolioId { get; set; }

    [Required(ErrorMessage = "Seleccione un artista")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista válido")]
    public int ArtistaId { get; set; }

    [ForeignKey("ArtistaId")]
    public Artistas? Artista { get; set; }

    [Required(ErrorMessage = "El título del portafolio es requerido")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Seleccione un estado")]
    public string Estado { get; set; } = "Publicado"; // Borrador, Pendiente de aprobación, Publicado, Rechazado

    public string? ImagenPortada { get; set; }

    public virtual ICollection<PortafoliosDetalle> PortafoliosDetalle { get; set; } = new List<PortafoliosDetalle>();
}