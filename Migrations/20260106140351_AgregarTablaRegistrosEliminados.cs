using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaRegistrosEliminados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registros_eliminados",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_tabla = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    registro_id = table.Column<int>(type: "integer", nullable: false),
                    datos_json = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_eliminacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    razon_eliminacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_eliminados", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registros_eliminados");
        }
    }
}
