using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTemaColorUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tema_color",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tema_color",
                table: "usuarios");
        }
    }
}
