using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class EliminarNivelImpactoEstadoDeODS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "nivel_impacto",
                table: "ods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "ods",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nivel_impacto",
                table: "ods",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
