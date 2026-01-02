using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class MigracionCompletaSistemaAlcaldia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departamentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departamentos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_completo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correo_electronico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contrasena_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "municipios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    departamento_id = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipios", x => x.id);
                    table.ForeignKey(
                        name: "FK_municipios_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alcaldias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    logo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    municipio_id = table.Column<int>(type: "integer", nullable: true),
                    departamento_id = table.Column<int>(type: "integer", nullable: true),
                    direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    correo_institucional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    alcalde_actual_id = table.Column<int>(type: "integer", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alcaldias", x => x.id);
                    table.ForeignKey(
                        name: "FK_alcaldias_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_alcaldias_municipios_municipio_id",
                        column: x => x.municipio_id,
                        principalTable: "municipios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "alcaldes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_documento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    numero_documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    periodo_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    periodo_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    slogan = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    partido_politico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    correo_electronico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alcaldes", x => x.id);
                    table.ForeignKey(
                        name: "FK_alcaldes_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lineas_estrategicas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    plan_dtp_id = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lineas_estrategicas", x => x.id);
                    table.ForeignKey(
                        name: "FK_lineas_estrategicas_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metas_ods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas_ods", x => x.id);
                    table.ForeignKey(
                        name: "FK_metas_ods_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "secretarias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    secretario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    correo_institucional = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secretarias", x => x.id);
                    table.ForeignKey(
                        name: "FK_secretarias_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vigencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    año = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vigencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_vigencias_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sectores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    linea_estrategica_id = table.Column<int>(type: "integer", nullable: false),
                    aplicacion = table.Column<string>(type: "text", nullable: true),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sectores", x => x.id);
                    table.ForeignKey(
                        name: "FK_sectores_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sectores_lineas_estrategicas_linea_estrategica_id",
                        column: x => x.linea_estrategica_id,
                        principalTable: "lineas_estrategicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    meta_ods_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ods", x => x.id);
                    table.ForeignKey(
                        name: "FK_ods_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ods_metas_ods_meta_ods_id",
                        column: x => x.meta_ods_id,
                        principalTable: "metas_ods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "responsables",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_documento = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    numero_identificacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tipo_responsable = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    secretaria_id = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responsables", x => x.id);
                    table.ForeignKey(
                        name: "FK_responsables_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_responsables_secretarias_secretaria_id",
                        column: x => x.secretaria_id,
                        principalTable: "secretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "subsecretarias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    secretaria_id = table.Column<int>(type: "integer", nullable: false),
                    responsable = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    correo_institucional = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subsecretarias", x => x.id);
                    table.ForeignKey(
                        name: "FK_subsecretarias_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subsecretarias_secretarias_secretaria_id",
                        column: x => x.secretaria_id,
                        principalTable: "secretarias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alcaldes_vigencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcalde_id = table.Column<int>(type: "integer", nullable: false),
                    vigencia_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alcaldes_vigencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_alcaldes_vigencias_alcaldes_alcalde_id",
                        column: x => x.alcalde_id,
                        principalTable: "alcaldes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alcaldes_vigencias_vigencias_vigencia_id",
                        column: x => x.vigencia_id,
                        principalTable: "vigencias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planes_departamentales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    departamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    eje = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sector_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes_departamentales", x => x.id);
                    table.ForeignKey(
                        name: "FK_planes_departamentales_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planes_departamentales_sectores_sector_id",
                        column: x => x.sector_id,
                        principalTable: "sectores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "planes_nacionales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    eje = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sector_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes_nacionales", x => x.id);
                    table.ForeignKey(
                        name: "FK_planes_nacionales_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planes_nacionales_sectores_sector_id",
                        column: x => x.sector_id,
                        principalTable: "sectores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "proyectos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    codigo_bpin = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    presupuesto_ejecutado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valor_proyecto = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    responsable_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado_proyecto = table.Column<bool>(type: "boolean", nullable: true),
                    porcentaje_avance = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyectos", x => x.id);
                    table.ForeignKey(
                        name: "FK_proyectos_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_proyectos_responsables_responsable_id",
                        column: x => x.responsable_id,
                        principalTable: "responsables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "planes_municipales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    municipio_id = table.Column<int>(type: "integer", nullable: true),
                    alcalde_id = table.Column<int>(type: "integer", nullable: true),
                    plan_nacional_id = table.Column<int>(type: "integer", nullable: true),
                    plan_dptl_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes_municipales", x => x.id);
                    table.ForeignKey(
                        name: "FK_planes_municipales_alcaldes_alcalde_id",
                        column: x => x.alcalde_id,
                        principalTable: "alcaldes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_planes_municipales_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planes_municipales_municipios_municipio_id",
                        column: x => x.municipio_id,
                        principalTable: "municipios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_planes_municipales_planes_departamentales_plan_dptl_id",
                        column: x => x.plan_dptl_id,
                        principalTable: "planes_departamentales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_planes_municipales_planes_nacionales_plan_nacional_id",
                        column: x => x.plan_nacional_id,
                        principalTable: "planes_nacionales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "actividades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    proyecto_id = table.Column<int>(type: "integer", nullable: false),
                    responsable_id = table.Column<int>(type: "integer", nullable: true),
                    vigencia_id = table.Column<int>(type: "integer", nullable: true),
                    tipo_actividad = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    fecha_inicio_programada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin_programada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_inicio_real = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin_real = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    presupuesto_ejecutado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    unidad_medida = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    meta_planeada = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    estado_actividad = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    porcentaje_avance = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades", x => x.id);
                    table.ForeignKey(
                        name: "FK_actividades_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_actividades_proyectos_proyecto_id",
                        column: x => x.proyecto_id,
                        principalTable: "proyectos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_actividades_responsables_responsable_id",
                        column: x => x.responsable_id,
                        principalTable: "responsables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_actividades_vigencias_vigencia_id",
                        column: x => x.vigencia_id,
                        principalTable: "vigencias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "programas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    presupuesto_cuatrienio = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    plan_municipal_id = table.Column<int>(type: "integer", nullable: true),
                    sector_id = table.Column<int>(type: "integer", nullable: false),
                    ods_id = table.Column<int>(type: "integer", nullable: true),
                    meta_resultado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programas", x => x.id);
                    table.ForeignKey(
                        name: "FK_programas_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programas_ods_ods_id",
                        column: x => x.ods_id,
                        principalTable: "ods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_programas_planes_municipales_plan_municipal_id",
                        column: x => x.plan_municipal_id,
                        principalTable: "planes_municipales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_programas_sectores_sector_id",
                        column: x => x.sector_id,
                        principalTable: "sectores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    actividad_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_evidencia = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    nombre_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    tipo_mime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tamano_bytes = table.Column<long>(type: "bigint", nullable: true),
                    hash_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ruta_almacenamiento = table.Column<string>(type: "text", nullable: true),
                    url_publica = table.Column<string>(type: "text", nullable: true),
                    fecha_evidencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ubicacion_captura = table.Column<string>(type: "text", nullable: true),
                    verificada = table.Column<int>(type: "integer", nullable: true),
                    verificada_por = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidencias_actividades_actividad_id",
                        column: x => x.actividad_id,
                        principalTable: "actividades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evidencias_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evidencias_usuarios_verificada_por",
                        column: x => x.verificada_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    programa_id = table.Column<int>(type: "integer", nullable: false),
                    presupuesto_asignado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.id);
                    table.ForeignKey(
                        name: "FK_productos_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productos_programas_programa_id",
                        column: x => x.programa_id,
                        principalTable: "programas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "indicadores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    unidad_medida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    linea_base = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    meta_final = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    responsable_id = table.Column<int>(type: "integer", nullable: true),
                    producto_id = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_indicadores", x => x.id);
                    table.ForeignKey(
                        name: "FK_indicadores_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_indicadores_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_indicadores_responsables_responsable_id",
                        column: x => x.responsable_id,
                        principalTable: "responsables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "proyectos_indicadores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alcaldia_id = table.Column<int>(type: "integer", nullable: false),
                    proyecto_id = table.Column<int>(type: "integer", nullable: false),
                    indicador_id = table.Column<int>(type: "integer", nullable: false),
                    porcentaje_avance = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyectos_indicadores", x => x.id);
                    table.ForeignKey(
                        name: "FK_proyectos_indicadores_alcaldias_alcaldia_id",
                        column: x => x.alcaldia_id,
                        principalTable: "alcaldias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_proyectos_indicadores_indicadores_indicador_id",
                        column: x => x.indicador_id,
                        principalTable: "indicadores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proyectos_indicadores_proyectos_proyecto_id",
                        column: x => x.proyecto_id,
                        principalTable: "proyectos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_alcaldia_id_codigo",
                table: "actividades",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_actividades_proyecto_id",
                table: "actividades",
                column: "proyecto_id");

            migrationBuilder.CreateIndex(
                name: "IX_actividades_responsable_id",
                table: "actividades",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "IX_actividades_vigencia_id",
                table: "actividades",
                column: "vigencia_id");

            migrationBuilder.CreateIndex(
                name: "IX_alcaldes_alcaldia_id",
                table: "alcaldes",
                column: "alcaldia_id");

            migrationBuilder.CreateIndex(
                name: "IX_alcaldes_numero_documento",
                table: "alcaldes",
                column: "numero_documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alcaldes_vigencias_alcalde_id_vigencia_id",
                table: "alcaldes_vigencias",
                columns: new[] { "alcalde_id", "vigencia_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alcaldes_vigencias_vigencia_id",
                table: "alcaldes_vigencias",
                column: "vigencia_id");

            migrationBuilder.CreateIndex(
                name: "IX_alcaldias_departamento_id",
                table: "alcaldias",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_alcaldias_municipio_id",
                table: "alcaldias",
                column: "municipio_id");

            migrationBuilder.CreateIndex(
                name: "IX_alcaldias_nit",
                table: "alcaldias",
                column: "nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_departamentos_codigo",
                table: "departamentos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_actividad_id",
                table: "evidencias",
                column: "actividad_id");

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_alcaldia_id_codigo",
                table: "evidencias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidencias_verificada_por",
                table: "evidencias",
                column: "verificada_por");

            migrationBuilder.CreateIndex(
                name: "IX_indicadores_alcaldia_id_codigo",
                table: "indicadores",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_indicadores_producto_id",
                table: "indicadores",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_indicadores_responsable_id",
                table: "indicadores",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "IX_lineas_estrategicas_alcaldia_id_codigo",
                table: "lineas_estrategicas",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metas_ods_alcaldia_id_codigo",
                table: "metas_ods",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_municipios_codigo",
                table: "municipios",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_municipios_departamento_id",
                table: "municipios",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_ods_alcaldia_id_codigo",
                table: "ods",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ods_meta_ods_id",
                table: "ods",
                column: "meta_ods_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_departamentales_alcaldia_id_codigo",
                table: "planes_departamentales",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planes_departamentales_sector_id",
                table: "planes_departamentales",
                column: "sector_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_municipales_alcalde_id",
                table: "planes_municipales",
                column: "alcalde_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_municipales_alcaldia_id_codigo",
                table: "planes_municipales",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planes_municipales_municipio_id",
                table: "planes_municipales",
                column: "municipio_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_municipales_plan_dptl_id",
                table: "planes_municipales",
                column: "plan_dptl_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_municipales_plan_nacional_id",
                table: "planes_municipales",
                column: "plan_nacional_id");

            migrationBuilder.CreateIndex(
                name: "IX_planes_nacionales_alcaldia_id_codigo",
                table: "planes_nacionales",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_planes_nacionales_sector_id",
                table: "planes_nacionales",
                column: "sector_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_alcaldia_id_codigo",
                table: "productos",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_productos_programa_id",
                table: "productos",
                column: "programa_id");

            migrationBuilder.CreateIndex(
                name: "IX_programas_alcaldia_id_codigo",
                table: "programas",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_programas_ods_id",
                table: "programas",
                column: "ods_id");

            migrationBuilder.CreateIndex(
                name: "IX_programas_plan_municipal_id",
                table: "programas",
                column: "plan_municipal_id");

            migrationBuilder.CreateIndex(
                name: "IX_programas_sector_id",
                table: "programas",
                column: "sector_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_alcaldia_id_codigo",
                table: "proyectos",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_responsable_id",
                table: "proyectos",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_indicadores_alcaldia_id",
                table: "proyectos_indicadores",
                column: "alcaldia_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_indicadores_indicador_id",
                table: "proyectos_indicadores",
                column: "indicador_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_indicadores_proyecto_id_indicador_id",
                table: "proyectos_indicadores",
                columns: new[] { "proyecto_id", "indicador_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_responsables_alcaldia_id",
                table: "responsables",
                column: "alcaldia_id");

            migrationBuilder.CreateIndex(
                name: "IX_responsables_numero_identificacion",
                table: "responsables",
                column: "numero_identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_responsables_secretaria_id",
                table: "responsables",
                column: "secretaria_id");

            migrationBuilder.CreateIndex(
                name: "IX_secretarias_alcaldia_id_codigo",
                table: "secretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sectores_alcaldia_id_codigo",
                table: "sectores",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sectores_linea_estrategica_id",
                table: "sectores",
                column: "linea_estrategica_id");

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_alcaldia_id_codigo",
                table: "subsecretarias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_secretaria_id",
                table: "subsecretarias",
                column: "secretaria_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo_electronico",
                table: "usuarios",
                column: "correo_electronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roles_rol_id",
                table: "usuarios_roles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roles_usuario_id_rol_id",
                table: "usuarios_roles",
                columns: new[] { "usuario_id", "rol_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vigencias_alcaldia_id_codigo",
                table: "vigencias",
                columns: new[] { "alcaldia_id", "codigo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alcaldes_vigencias");

            migrationBuilder.DropTable(
                name: "evidencias");

            migrationBuilder.DropTable(
                name: "proyectos_indicadores");

            migrationBuilder.DropTable(
                name: "subsecretarias");

            migrationBuilder.DropTable(
                name: "usuarios_roles");

            migrationBuilder.DropTable(
                name: "actividades");

            migrationBuilder.DropTable(
                name: "indicadores");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "proyectos");

            migrationBuilder.DropTable(
                name: "vigencias");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "responsables");

            migrationBuilder.DropTable(
                name: "programas");

            migrationBuilder.DropTable(
                name: "secretarias");

            migrationBuilder.DropTable(
                name: "ods");

            migrationBuilder.DropTable(
                name: "planes_municipales");

            migrationBuilder.DropTable(
                name: "metas_ods");

            migrationBuilder.DropTable(
                name: "alcaldes");

            migrationBuilder.DropTable(
                name: "planes_departamentales");

            migrationBuilder.DropTable(
                name: "planes_nacionales");

            migrationBuilder.DropTable(
                name: "sectores");

            migrationBuilder.DropTable(
                name: "lineas_estrategicas");

            migrationBuilder.DropTable(
                name: "alcaldias");

            migrationBuilder.DropTable(
                name: "municipios");

            migrationBuilder.DropTable(
                name: "departamentos");
        }
    }
}
