using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261123175736_ActualizarPortafolios : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NotaCuratorial",
            table: "PortafoliosDetalle");

        migrationBuilder.DropColumn(
            name: "Estado",
            table: "Portafolios");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "NotaCuratorial",
            table: "PortafoliosDetalle",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Estado",
            table: "Portafolios",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }
}
