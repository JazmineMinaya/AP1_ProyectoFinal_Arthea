using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261126233730_ActualizarPedidosServicios : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Facturas_Pedidos_PedidoId",
            table: "Facturas");

        migrationBuilder.DropForeignKey(
            name: "FK_PedidosDetalles_Servicios_ServicioId",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "DireccionEntrega",
            table: "Pedidos");

        migrationBuilder.AddColumn<string>(
            name: "ContactoEntrega",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DireccionEntrega",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "EnlaceReferencia",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "EstadoAprobacion",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            name: "FechaRespuesta",
            table: "PedidosDetalles",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GuiaRequerimientos",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "MedioEntrega",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ModalidadEntrega",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "MotivoRechazo",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Requerimientos",
            table: "PedidosDetalles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            name: "FechaConfirmacion",
            table: "Pedidos",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "FechaLimiteAprobacion",
            table: "Pedidos",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.CreateTable(
            name: "PedidosReferencias",
            columns: table => new
            {
                ReferenciaId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DetalleId = table.Column<int>(type: "int", nullable: false),
                Imagen = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PedidosReferencias", x => x.ReferenciaId);
                table.ForeignKey(
                    name: "FK_PedidosReferencias_PedidosDetalles_DetalleId",
                    column: x => x.DetalleId,
                    principalTable: "PedidosDetalles",
                    principalColumn: "DetalleId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PedidosReferencias_DetalleId",
            table: "PedidosReferencias",
            column: "DetalleId");

        migrationBuilder.AddForeignKey(
            name: "FK_Facturas_Pedidos_PedidoId",
            table: "Facturas",
            column: "PedidoId",
            principalTable: "Pedidos",
            principalColumn: "PedidoId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_PedidosDetalles_Servicios_ServicioId",
            table: "PedidosDetalles",
            column: "ServicioId",
            principalTable: "Servicios",
            principalColumn: "ServicioId",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Facturas_Pedidos_PedidoId",
            table: "Facturas");

        migrationBuilder.DropForeignKey(
            name: "FK_PedidosDetalles_Servicios_ServicioId",
            table: "PedidosDetalles");

        migrationBuilder.DropTable(
            name: "PedidosReferencias");

        migrationBuilder.DropColumn(
            name: "ContactoEntrega",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "DireccionEntrega",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "EnlaceReferencia",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "EstadoAprobacion",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "FechaRespuesta",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "GuiaRequerimientos",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "MedioEntrega",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "ModalidadEntrega",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "MotivoRechazo",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "Requerimientos",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "FechaConfirmacion",
            table: "Pedidos");

        migrationBuilder.DropColumn(
            name: "FechaLimiteAprobacion",
            table: "Pedidos");

        migrationBuilder.AddColumn<string>(
            name: "DireccionEntrega",
            table: "Pedidos",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddForeignKey(
            name: "FK_Facturas_Pedidos_PedidoId",
            table: "Facturas",
            column: "PedidoId",
            principalTable: "Pedidos",
            principalColumn: "PedidoId",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_PedidosDetalles_Servicios_ServicioId",
            table: "PedidosDetalles",
            column: "ServicioId",
            principalTable: "Servicios",
            principalColumn: "ServicioId",
            onDelete: ReferentialAction.Cascade);
    }
}
