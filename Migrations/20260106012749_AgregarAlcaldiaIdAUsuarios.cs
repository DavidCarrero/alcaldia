using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlcaldiaIdAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "alcaldia_id",
                table: "usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_alcaldia_id",
                table: "usuarios",
                column: "alcaldia_id");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_alcaldias_alcaldia_id",
                table: "usuarios",
                column: "alcaldia_id",
                principalTable: "alcaldias",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_alcaldias_alcaldia_id",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_alcaldia_id",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "alcaldia_id",
                table: "usuarios");
        }
    }
}
