using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class ConvertirSubsecretariasSecretariasMuchosAMuchos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subsecretarias_secretarias_secretaria_id",
                table: "subsecretarias");

            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_secretaria_id",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "secretaria_id",
                table: "subsecretarias");

            migrationBuilder.CreateTable(
                name: "secretarias_subsecretarias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    secretaria_id = table.Column<int>(type: "integer", nullable: false),
                    subsecretaria_id = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secretarias_subsecretarias", x => x.id);
                    table.ForeignKey(
                        name: "FK_secretarias_subsecretarias_secretarias_secretaria_id",
                        column: x => x.secretaria_id,
                        principalTable: "secretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_secretarias_subsecretarias_subsecretarias_subsecretaria_id",
                        column: x => x.subsecretaria_id,
                        principalTable: "subsecretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id",
                table: "secretarias_subsecretarias",
                columns: new[] { "secretaria_id", "subsecretaria_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_secretarias_subsecretarias_subsecretaria_id",
                table: "secretarias_subsecretarias",
                column: "subsecretaria_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "secretarias_subsecretarias");

            migrationBuilder.AddColumn<int>(
                name: "secretaria_id",
                table: "subsecretarias",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_secretaria_id",
                table: "subsecretarias",
                column: "secretaria_id");

            migrationBuilder.AddForeignKey(
                name: "FK_subsecretarias_secretarias_secretaria_id",
                table: "subsecretarias",
                column: "secretaria_id",
                principalTable: "secretarias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
