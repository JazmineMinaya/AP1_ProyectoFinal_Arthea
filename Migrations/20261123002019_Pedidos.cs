using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261123002019_Pedidos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Pedidos",
            columns: table => new
            {
                PedidoId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                ClienteId = table.Column<int>(type: "int", nullable: false),
                ArtistaId = table.Column<int>(type: "int", nullable: false),
                DireccionEntrega = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EstadoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FechaPago = table.Column<DateTime>(type: "datetime2", nullable: true),
                Total = table.Column<double>(type: "float", nullable: false),
                FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: true),
                FechaCancelacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                MotivoCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                MontoReembolsado = table.Column<double>(type: "float", nullable: false),
                FechaReembolso = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pedidos", x => x.PedidoId);
                table.ForeignKey(
                    name: "FK_Pedidos_Artistas_ArtistaId",
                    column: x => x.ArtistaId,
                    principalTable: "Artistas",
                    principalColumn: "ArtistaId");
                table.ForeignKey(
                    name: "FK_Pedidos_Clientes_ClienteId",
                    column: x => x.ClienteId,
                    principalTable: "Clientes",
                    principalColumn: "ClienteId");
            });

        migrationBuilder.CreateTable(
            name: "Facturas",
            columns: table => new
            {
                FacturaId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PedidoId = table.Column<int>(type: "int", nullable: false),
                Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                NumeroFactura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Total = table.Column<double>(type: "float", nullable: false),
                Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Facturas", x => x.FacturaId);
                table.ForeignKey(
                    name: "FK_Facturas_Pedidos_PedidoId",
                    column: x => x.PedidoId,
                    principalTable: "Pedidos",
                    principalColumn: "PedidoId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PedidosDetalles",
            columns: table => new
            {
                DetalleId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PedidoId = table.Column<int>(type: "int", nullable: false),
                ServicioId = table.Column<int>(type: "int", nullable: false),
                Cantidad = table.Column<int>(type: "int", nullable: false),
                Precio = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PedidosDetalles", x => x.DetalleId);
                table.ForeignKey(
                    name: "FK_PedidosDetalles_Pedidos_PedidoId",
                    column: x => x.PedidoId,
                    principalTable: "Pedidos",
                    principalColumn: "PedidoId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PedidosDetalles_Servicios_ServicioId",
                    column: x => x.ServicioId,
                    principalTable: "Servicios",
                    principalColumn: "ServicioId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Facturas_PedidoId",
            table: "Facturas",
            column: "PedidoId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Pedidos_ArtistaId",
            table: "Pedidos",
            column: "ArtistaId");

        migrationBuilder.CreateIndex(
            name: "IX_Pedidos_ClienteId",
            table: "Pedidos",
            column: "ClienteId");

        migrationBuilder.CreateIndex(
            name: "IX_PedidosDetalles_PedidoId",
            table: "PedidosDetalles",
            column: "PedidoId");

        migrationBuilder.CreateIndex(
            name: "IX_PedidosDetalles_ServicioId",
            table: "PedidosDetalles",
            column: "ServicioId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Facturas");

        migrationBuilder.DropTable(
            name: "PedidosDetalles");

        migrationBuilder.DropTable(
            name: "Pedidos");
    }
}
