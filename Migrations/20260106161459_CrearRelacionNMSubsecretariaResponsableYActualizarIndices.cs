using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class CrearRelacionNMSubsecretariaResponsableYActualizarIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_responsables_secretarias_secretaria_id",
                table: "responsables");

            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_alcaldia_id_codigo",
                table: "subsecretarias");

            migrationBuilder.DropIndex(
                name: "IX_secretarias_alcaldia_id_codigo",
                table: "secretarias");

            migrationBuilder.RenameColumn(
                name: "secretaria_id",
                table: "responsables",
                newName: "SecretariaId");

            migrationBuilder.RenameIndex(
                name: "IX_responsables_secretaria_id",
                table: "responsables",
                newName: "IX_responsables_SecretariaId");

            migrationBuilder.CreateTable(
                name: "subsecretarias_responsables",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subsecretaria_id = table.Column<int>(type: "integer", nullable: false),
                    responsable_id = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsecretarias_responsables", x => x.id);
                    table.ForeignKey(
                        name: "FK_subsecretarias_responsables_responsables_responsable_id",
                        column: x => x.responsable_id,
                        principalTable: "responsables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_subsecretarias_responsables_subsecretarias_subsecretaria_id",
                        column: x => x.subsecretaria_id,
                        principalTable: "subsecretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_alcaldia_id_codigo",
                table: "subsecretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_secretarias_alcaldia_id_codigo",
                table: "secretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_responsables_responsable_id",
                table: "subsecretarias_responsables",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_responsables_subsecretaria_id_responsable_id",
                table: "subsecretarias_responsables",
                columns: new[] { "subsecretaria_id", "responsable_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_responsables_secretarias_SecretariaId",
                table: "responsables",
                column: "SecretariaId",
                principalTable: "secretarias",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_responsables_secretarias_SecretariaId",
                table: "responsables");

            migrationBuilder.DropTable(
                name: "subsecretarias_responsables");

            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_alcaldia_id_codigo",
                table: "subsecretarias");

            migrationBuilder.DropIndex(
                name: "IX_secretarias_alcaldia_id_codigo",
                table: "secretarias");

            migrationBuilder.RenameColumn(
                name: "SecretariaId",
                table: "responsables",
                newName: "secretaria_id");

            migrationBuilder.RenameIndex(
                name: "IX_responsables_SecretariaId",
                table: "responsables",
                newName: "IX_responsables_secretaria_id");

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_alcaldia_id_codigo",
                table: "subsecretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_secretarias_alcaldia_id_codigo",
                table: "secretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_responsables_secretarias_secretaria_id",
                table: "responsables",
                column: "secretaria_id",
                principalTable: "secretarias",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
