using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261121140243_Artistas : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Artistas",
            columns: table => new
            {
                ArtistaId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Disciplina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Biografia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Artistas", x => x.ArtistaId);
                table.ForeignKey(
                    name: "FK_Artistas_AspNetUsers_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Artistas_UsuarioId",
            table: "Artistas",
            column: "UsuarioId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Artistas");
    }
}
