using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColumnasEliminacionSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "vigencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "vigencias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "vigencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "usuarios_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "usuarios_roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "usuarios_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "subsecretarias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "subsecretarias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "subsecretarias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "sectores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "sectores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "sectores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "secretarias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "secretarias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "secretarias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "responsables",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "responsables",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "responsables",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "proyectos_indicadores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "proyectos_indicadores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "proyectos_indicadores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "proyectos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "proyectos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "proyectos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "programas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "programas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "programas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "productos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "productos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "planes_nacionales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "planes_nacionales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "planes_nacionales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "planes_municipales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "planes_municipales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "planes_municipales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "planes_departamentales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "planes_departamentales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "planes_departamentales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ods_metas_ods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ods_metas_ods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ods_metas_ods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "municipios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "municipios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "municipios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "metas_ods",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "metas_ods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "metas_ods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "lineas_estrategicas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "lineas_estrategicas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "lineas_estrategicas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "indicadores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "indicadores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "indicadores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "evidencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "evidencias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "evidencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "departamentos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "departamentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "departamentos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "alcaldias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "alcaldias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "alcaldias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "alcaldes_vigencias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "alcaldes_vigencias",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "alcaldes_vigencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "alcaldes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "alcaldes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "alcaldes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "actividades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "actividades",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "actividades",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "vigencias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "vigencias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "vigencias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "usuarios_roles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "usuarios_roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "usuarios_roles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "subsecretarias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "sectores");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "sectores");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "sectores");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "secretarias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "secretarias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "secretarias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "responsables");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "responsables");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "responsables");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "proyectos_indicadores");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "proyectos_indicadores");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "proyectos_indicadores");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "proyectos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "proyectos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "proyectos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "programas");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "programas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "programas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "planes_nacionales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "planes_nacionales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "planes_nacionales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "planes_municipales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "planes_municipales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "planes_municipales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "planes_departamentales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "planes_departamentales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "planes_departamentales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ods_metas_ods");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ods");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "municipios");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "municipios");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "municipios");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "metas_ods");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "metas_ods");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "metas_ods");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "lineas_estrategicas");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "lineas_estrategicas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "lineas_estrategicas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "indicadores");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "indicadores");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "indicadores");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "evidencias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "evidencias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "evidencias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "departamentos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "departamentos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "departamentos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "alcaldias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "alcaldias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "alcaldias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "alcaldes_vigencias");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "alcaldes_vigencias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "alcaldes_vigencias");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "alcaldes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "alcaldes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "alcaldes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "actividades");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "actividades");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "actividades");
        }
    }
}
