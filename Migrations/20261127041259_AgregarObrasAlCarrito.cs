using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261127041259_AgregarObrasAlCarrito : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "ServicioId",
            table: "PedidosDetalles",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<int>(
            name: "ObraId",
            table: "PedidosDetalles",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_PedidosDetalles_ObraId",
            table: "PedidosDetalles",
            column: "ObraId");

        migrationBuilder.AddCheckConstraint(
            name: "CK_PedidosDetalle_Tipo",
            table: "PedidosDetalles",
            sql: "([ServicioId] IS NOT NULL AND [ObraId] IS NULL) OR ([ServicioId] IS NULL AND [ObraId] IS NOT NULL)");

        migrationBuilder.AddForeignKey(
            name: "FK_PedidosDetalles_Obras_ObraId",
            table: "PedidosDetalles",
            column: "ObraId",
            principalTable: "Obras",
            principalColumn: "ObraId",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_PedidosDetalles_Obras_ObraId",
            table: "PedidosDetalles");

        migrationBuilder.DropIndex(
            name: "IX_PedidosDetalles_ObraId",
            table: "PedidosDetalles");

        migrationBuilder.DropCheckConstraint(
            name: "CK_PedidosDetalle_Tipo",
            table: "PedidosDetalles");

        migrationBuilder.DropColumn(
            name: "ObraId",
            table: "PedidosDetalles");

        migrationBuilder.AlterColumn<int>(
            name: "ServicioId",
            table: "PedidosDetalles",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }
}
