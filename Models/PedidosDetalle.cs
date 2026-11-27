using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public partial class PedidosDetalle
{
    [Key]
    public int DetalleId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un pedido")]
    public int PedidoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un servicio")]
    public int ServicioId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Ingrese una cantidad válida")]
    public int Cantidad { get; set; } = 1;

    [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un precio válido")]
    public double Precio { get; set; }

    [NotMapped]
    public double Subtotal => Cantidad * Precio;

    [ForeignKey("PedidoId")]
    public virtual Pedidos? Pedido { get; set; }

    [ForeignKey("ServicioId")]
    public virtual Servicios? Servicio { get; set; }
}