using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public partial class Pedidos
{
    [Key]
    public int PedidoId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un artista")]
    public int ArtistaId { get; set; }

    [Required(ErrorMessage = "Ingrese la dirección de entrega")]
    public string DireccionEntrega { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione el estado del pedido")]
    public string Estado { get; set; } = "Pendiente";

    public string EstadoPago { get; set; } = "Pendiente";

    public DateTime? FechaPago { get; set; }

    public double Total { get; set; }

    public DateTime? FechaRecepcion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public string? MotivoCancelacion { get; set; }

    public double MontoReembolsado { get; set; }

    public DateTime? FechaReembolso { get; set; }

    [ForeignKey("ClienteId")]
    public virtual Clientes? Cliente { get; set; }

    [ForeignKey("ArtistaId")]
    public virtual Artistas? Artista { get; set; }

    public virtual ICollection<PedidosDetalle> PedidosDetalle { get; set; } = new List<PedidosDetalle>();

    public virtual Facturas? Factura { get; set; }
}
