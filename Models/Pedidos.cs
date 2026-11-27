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

    [Required(ErrorMessage = "Seleccione el estado del pedido")]
    public string Estado { get; set; } = "Pendiente";

    public string EstadoPago { get; set; } = "Pendiente";

    public DateTime FechaLimiteAprobacion { get; set; }

    public DateTime? FechaConfirmacion { get; set; }

    public DateTime? FechaPago { get; set; }

    public double Total { get; set; }

    public DateTime? FechaRecepcion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public string? MotivoCancelacion { get; set; }

    public double MontoReembolsado { get; set; }

    public DateTime? FechaReembolso { get; set; }

    [ForeignKey("ClienteId")]
    public virtual Clientes? Cliente { get; set; }

    public virtual ICollection<PedidosDetalle> PedidosDetalle { get; set; } = new List<PedidosDetalle>();

    public virtual Facturas? Factura { get; set; }
}
