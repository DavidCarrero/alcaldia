using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class ConvertirMunicipiosDepartamentosMuchosAMuchos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_municipios_departamentos_departamento_id",
                table: "municipios");

            migrationBuilder.DropIndex(
                name: "IX_municipios_departamento_id",
                table: "municipios");

            migrationBuilder.DropColumn(
                name: "departamento_id",
                table: "municipios");

            migrationBuilder.CreateTable(
                name: "municipio_departamentos",
                columns: table => new
                {
                    municipio_id = table.Column<int>(type: "integer", nullable: false),
                    departamento_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipio_departamentos", x => new { x.municipio_id, x.departamento_id });
                    table.ForeignKey(
                        name: "FK_municipio_departamentos_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_municipio_departamentos_municipios_municipio_id",
                        column: x => x.municipio_id,
                        principalTable: "municipios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_municipio_departamentos_departamento_id",
                table: "municipio_departamentos",
                column: "departamento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "municipio_departamentos");

            migrationBuilder.AddColumn<int>(
                name: "departamento_id",
                table: "municipios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_municipios_departamento_id",
                table: "municipios",
                column: "departamento_id");

            migrationBuilder.AddForeignKey(
                name: "FK_municipios_departamentos_departamento_id",
                table: "municipios",
                column: "departamento_id",
                principalTable: "departamentos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
