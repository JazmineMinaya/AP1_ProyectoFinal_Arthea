using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261124234555_ActualizarPortafolio : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "ImagenPortada",
            table: "Portafolios",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "ImagenPortada",
            table: "Portafolios",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
