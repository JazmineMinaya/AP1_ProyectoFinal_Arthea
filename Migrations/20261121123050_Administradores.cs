using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261121123050_Administradores : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Usuarios",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Nombres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                Estado = table.Column<bool>(type: "bit", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Usuarios", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Administradores",
            columns: table => new
            {
                AdministradorId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Administradores", x => x.AdministradorId);
                table.ForeignKey(
                    name: "FK_Administradores_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Administradores_UsuarioId",
            table: "Administradores",
            column: "UsuarioId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Administradores");

        migrationBuilder.DropTable(
            name: "Usuarios");
    }
}
