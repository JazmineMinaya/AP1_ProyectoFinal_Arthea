using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class PortafoliosDetalle
{
    [Key]
    public int DetalleId { get; set; }

    public int PortafolioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una obra")]
    public int ObraId { get; set; }

    [ForeignKey("PortafolioId")]
    public virtual Portafolios? Portafolio { get; set; }

    [ForeignKey("ObraId")]
    public virtual Obras? Obra { get; set; }
}