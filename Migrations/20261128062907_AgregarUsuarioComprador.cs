using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP1_ProyectoFinal_Arthea.Migrations;

public partial class _20261128062907_AgregarUsuarioComprador : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "ClienteId",
            table: "Pedidos",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<string>(
            name: "UsuarioId",
            table: "Pedidos",
            type: "nvarchar(450)",
            nullable: true);

        // Asociar los pedidos existentes con sus usuarios de Identity.
        migrationBuilder.Sql(@"
            UPDATE p
            SET p.UsuarioId = c.UsuarioId
            FROM Pedidos AS p
            INNER JOIN Clientes AS c ON p.ClienteId = c.ClienteId
            INNER JOIN AspNetUsers AS u ON c.UsuarioId = u.Id
            WHERE p.UsuarioId IS NULL;
        ");

        migrationBuilder.CreateIndex(
            name: "IX_Pedidos_UsuarioId",
            table: "Pedidos",
            column: "UsuarioId");

        migrationBuilder.AddForeignKey(
            name: "FK_Pedidos_AspNetUsers_UsuarioId",
            table: "Pedidos",
            column: "UsuarioId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Pedidos_AspNetUsers_UsuarioId",
            table: "Pedidos");

        migrationBuilder.DropIndex(
            name: "IX_Pedidos_UsuarioId",
            table: "Pedidos");

        migrationBuilder.DropColumn(
            name: "UsuarioId",
            table: "Pedidos");

        migrationBuilder.AlterColumn<int>(
            name: "ClienteId",
            table: "Pedidos",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }
}
