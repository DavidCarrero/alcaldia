using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("secretarias")]
public class Secretaria : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("alcaldia_id")]
    public int AlcaldiaId { get; set; }

    [ForeignKey("AlcaldiaId")]
    public Alcaldia Alcaldia { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [MaxLength(20)]
    [Column("sigla")]
    public string? Sigla { get; set; }

    [MaxLength(200)]
    [Column("secretario")]
    public string? Secretario { get; set; }

    [MaxLength(200)]
    [Column("correo_institucional")]
    public string? CorreoInstitucional { get; set; }

    [MaxLength(50)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("presupuesto_asignado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoAsignado { get; set; }

    // Relaciones
    public ICollection<Subsecretaria> Subsecretarias { get; set; } = new List<Subsecretaria>();
    public ICollection<Responsable> Responsables { get; set; } = new List<Responsable>();
}
