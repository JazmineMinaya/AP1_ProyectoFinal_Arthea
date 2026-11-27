using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

/// <inheritdoc />
public partial class _20261122161305_Servicios : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Servicios",
            columns: table => new
            {
                ServicioId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ArtistaId = table.Column<int>(type: "int", nullable: false),
                Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Imagen1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Imagen2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Imagen3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Imagen4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Imagen5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Estado = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Servicios", x => x.ServicioId);
                table.ForeignKey(
                    name: "FK_Servicios_Artistas_ArtistaId",
                    column: x => x.ArtistaId,
                    principalTable: "Artistas",
                    principalColumn: "ArtistaId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Servicios_ArtistaId",
            table: "Servicios",
            column: "ArtistaId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Servicios");
    }
}
