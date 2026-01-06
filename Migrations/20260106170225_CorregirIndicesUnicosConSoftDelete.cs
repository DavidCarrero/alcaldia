using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_alcaldia.Migrations
{
    /// <inheritdoc />
    public partial class CorregirIndicesUnicosConSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar índices únicos existentes
            migrationBuilder.DropIndex(
                name: "IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id",
                table: "secretarias_subsecretarias");
            
            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_responsables_subsecretaria_id_responsable_id",
                table: "subsecretarias_responsables");

            // Crear índices únicos parciales que excluyan registros eliminados
            // Solo se permite duplicados si IsDeleted = true
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id 
                ON secretarias_subsecretarias (secretaria_id, subsecretaria_id) 
                WHERE ""IsDeleted"" = false;
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_subsecretarias_responsables_subsecretaria_id_responsable_id 
                ON subsecretarias_responsables (subsecretaria_id, responsable_id) 
                WHERE ""IsDeleted"" = false;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar índices parciales
            migrationBuilder.DropIndex(
                name: "IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id",
                table: "secretarias_subsecretarias");
            
            migrationBuilder.DropIndex(
                name: "IX_subsecretarias_responsables_subsecretaria_id_responsable_id",
                table: "subsecretarias_responsables");

            // Recrear índices únicos originales (sin filtro)
            migrationBuilder.CreateIndex(
                name: "IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id",
                table: "secretarias_subsecretarias",
                columns: new[] { "secretaria_id", "subsecretaria_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subsecretarias_responsables_subsecretaria_id_responsable_id",
                table: "subsecretarias_responsables",
                columns: new[] { "subsecretaria_id", "responsable_id" },
                unique: true);
        }
    }
}
