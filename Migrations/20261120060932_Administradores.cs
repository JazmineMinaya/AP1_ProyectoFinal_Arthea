using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261120060932_Administradores : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Administradores",
            columns: table => new
            {
                AdministradorId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                Activo = table.Column<bool>(type: "bit", nullable: false),
                FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Administradores", x => x.AdministradorId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Administradores");
    }
}
