using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261125063752_EliminarArtistaDePedidos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Pedidos_Artistas_ArtistaId",
            table: "Pedidos");

        migrationBuilder.DropIndex(
            name: "IX_Pedidos_ArtistaId",
            table: "Pedidos");

        migrationBuilder.DropColumn(
            name: "ArtistaId",
            table: "Pedidos");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ArtistaId",
            table: "Pedidos",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Pedidos_ArtistaId",
            table: "Pedidos",
            column: "ArtistaId");

        migrationBuilder.AddForeignKey(
            name: "FK_Pedidos_Artistas_ArtistaId",
            table: "Pedidos",
            column: "ArtistaId",
            principalTable: "Artistas",
            principalColumn: "ArtistaId");
    }
}
