using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarEntidadesConSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ods_metas_ods",
                table: "ods_metas_ods");

            migrationBuilder.RenameColumn(
                name: "activo",
                table: "usuarios_roles",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "activo",
                table: "alcaldes_vigencias",
                newName: "Activo");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "usuarios_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "usuarios_roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "ods_metas_ods",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "ods_metas_ods",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ods_metas_ods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ods_metas_ods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "alcaldes_vigencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "alcaldes_vigencias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ods_metas_ods",
                table: "ods_metas_ods",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_ods_metas_ods_ods_id_meta_ods_id",
                table: "ods_metas_ods",
                columns: new[] { "ods_id", "meta_ods_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ods_metas_ods",
                table: "ods_metas_ods");

            migrationBuilder.DropIndex(
                name: "IX_ods_metas_ods_ods_id_meta_ods_id",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "usuarios_roles");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "usuarios_roles");

            migrationBuilder.DropColumn(
                name: "id",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "alcaldes_vigencias");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "alcaldes_vigencias");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "usuarios_roles",
                newName: "activo");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "alcaldes_vigencias",
                newName: "activo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ods_metas_ods",
                table: "ods_metas_ods",
                columns: new[] { "ods_id", "meta_ods_id" });
        }
    }
}
