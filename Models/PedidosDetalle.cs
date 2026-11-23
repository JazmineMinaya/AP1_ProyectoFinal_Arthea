using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public partial class PedidosDetalle
{
    [Key]
    public int DetalleId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un pedido")]
    public int PedidoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un servicio")]
    public int ServicioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; } = 1;

    [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
    public double Precio { get; set; }

    [NotMapped]
    public double Subtotal => Cantidad * Precio;

    [ForeignKey("PedidoId")]
    public virtual Pedidos? Pedido { get; set; }

    [ForeignKey("ServicioId")]
    public virtual Servicios? Servicio { get; set; }
}