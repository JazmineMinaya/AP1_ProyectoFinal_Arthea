using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP1_ProyectoFinal_Arthea.Models;

public partial class Facturas
{
    [Key]
    public int FacturaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un pedido")]
    public int PedidoId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Este campo es requerido")]
    public string NumeroFactura { get; set; } = string.Empty;

    public double Total { get; set; }

    public string Estado { get; set; } = "Pendiente";

    [ForeignKey("PedidoId")]
    public virtual Pedidos? Pedido { get; set; }
}
