using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class RefactorODSManyToManyMetasODS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ods_metas_ods_meta_ods_id",
                table: "ods");

            migrationBuilder.DropIndex(
                name: "IX_ods_meta_ods_id",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "meta_ods_id",
                table: "ods");

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

            migrationBuilder.CreateTable(
                name: "ods_metas_ods",
                columns: table => new
                {
                    ods_id = table.Column<int>(type: "integer", nullable: false),
                    meta_ods_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_asociacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ods_metas_ods", x => new { x.ods_id, x.meta_ods_id });
                    table.ForeignKey(
                        name: "FK_ods_metas_ods_metas_ods_meta_ods_id",
                        column: x => x.meta_ods_id,
                        principalTable: "metas_ods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ods_metas_ods_ods_ods_id",
                        column: x => x.ods_id,
                        principalTable: "ods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ods_metas_ods_meta_ods_id",
                table: "ods_metas_ods",
                column: "meta_ods_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "nivel_impacto",
                table: "ods");

            migrationBuilder.AddColumn<int>(
                name: "meta_ods_id",
                table: "ods",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ods_meta_ods_id",
                table: "ods",
                column: "meta_ods_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ods_metas_ods_meta_ods_id",
                table: "ods",
                column: "meta_ods_id",
                principalTable: "metas_ods",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
