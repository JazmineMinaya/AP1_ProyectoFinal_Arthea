using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public partial class PedidosDetalle
{
    [Key]
    public int DetalleId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un pedido")]
    public int PedidoId { get; set; }

    public int? ServicioId { get; set; }

    public int? ObraId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Ingrese una cantidad válida")]
    public int Cantidad { get; set; } = 1;

    [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un precio válido")]
    public double Precio { get; set; }

    [NotMapped]
    public double Subtotal => Cantidad * Precio;

    public string Requerimientos { get; set; } = string.Empty;

    public string GuiaRequerimientos { get; set; } = string.Empty;

    public string? EnlaceReferencia { get; set; }

    public string ModalidadEntrega { get; set; } = string.Empty;

    public string? DireccionEntrega { get; set; }

    public string? MedioEntrega { get; set; }

    public string? ContactoEntrega { get; set; }

    public string EstadoAprobacion { get; set; } = "Pendiente";

    public DateTime? FechaRespuesta { get; set; }

    public string? MotivoRechazo { get; set; }

    [ForeignKey("PedidoId")]
    public virtual Pedidos? Pedido { get; set; }

    [ForeignKey("ServicioId")]
    public virtual Servicios? Servicio { get; set; }

    [ForeignKey("ObraId")]
    public virtual Obras? Obra { get; set; }

    public virtual ICollection<PedidosReferencias> Referencias { get; set; } = new List<PedidosReferencias>();
}
