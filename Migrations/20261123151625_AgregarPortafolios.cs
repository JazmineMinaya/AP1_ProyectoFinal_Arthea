using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261123151625_AgregarPortafolios : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Portafolios",
            columns: table => new
            {
                PortafolioId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ArtistaId = table.Column<int>(type: "int", nullable: false),
                Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ImagenPortada = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Portafolios", x => x.PortafolioId);
                table.ForeignKey(
                    name: "FK_Portafolios_Artistas_ArtistaId",
                    column: x => x.ArtistaId,
                    principalTable: "Artistas",
                    principalColumn: "ArtistaId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PortafoliosDetalle",
            columns: table => new
            {
                DetalleId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PortafolioId = table.Column<int>(type: "int", nullable: false),
                ObraId = table.Column<int>(type: "int", nullable: false),
                NotaCuratorial = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PortafoliosDetalle", x => x.DetalleId);
                table.ForeignKey(
                    name: "FK_PortafoliosDetalle_Obras_ObraId",
                    column: x => x.ObraId,
                    principalTable: "Obras",
                    principalColumn: "ObraId",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PortafoliosDetalle_Portafolios_PortafolioId",
                    column: x => x.PortafolioId,
                    principalTable: "Portafolios",
                    principalColumn: "PortafolioId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Portafolios_ArtistaId",
            table: "Portafolios",
            column: "ArtistaId");

        migrationBuilder.CreateIndex(
            name: "IX_PortafoliosDetalle_ObraId",
            table: "PortafoliosDetalle",
            column: "ObraId");

        migrationBuilder.CreateIndex(
            name: "IX_PortafoliosDetalle_PortafolioId",
            table: "PortafoliosDetalle",
            column: "PortafolioId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PortafoliosDetalle");

        migrationBuilder.DropTable(
            name: "Portafolios");
    }
}
