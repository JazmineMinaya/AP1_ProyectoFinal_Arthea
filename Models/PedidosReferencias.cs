using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public class PedidosReferencias
{
    [Key]
    public int ReferenciaId { get; set; }

    public int DetalleId { get; set; }

    [Required]
    public string Imagen { get; set; } = string.Empty;

    [ForeignKey("DetalleId")]
    public virtual PedidosDetalle? Detalle { get; set; }
}
