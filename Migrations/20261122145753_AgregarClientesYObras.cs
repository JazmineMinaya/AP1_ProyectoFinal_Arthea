using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261122145753_AgregarClientesYObras : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Clientes",
            columns: table => new
            {
                ClienteId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                table.ForeignKey(
                    name: "FK_Clientes_AspNetUsers_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Obras",
            columns: table => new
            {
                ObraId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ArtistaId = table.Column<int>(type: "int", nullable: false),
                Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Obras", x => x.ObraId);
                table.ForeignKey(
                    name: "FK_Obras_Artistas_ArtistaId",
                    column: x => x.ArtistaId,
                    principalTable: "Artistas",
                    principalColumn: "ArtistaId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Clientes_UsuarioId",
            table: "Clientes",
            column: "UsuarioId");

        migrationBuilder.CreateIndex(
            name: "IX_Obras_ArtistaId",
            table: "Obras",
            column: "ArtistaId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Clientes");

        migrationBuilder.DropTable(
            name: "Obras");
    }
}
