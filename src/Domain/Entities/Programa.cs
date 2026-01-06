using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("programas")]
public class Programa : BaseEntity
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
    [MaxLength(200)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column("presupuesto_cuatrienio", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoCuatrienio { get; set; }

    [Column("plan_municipal_id")]
    public int? PlanMunicipalId { get; set; }

    [ForeignKey("PlanMunicipalId")]
    public PlanMunicipal? PlanMunicipal { get; set; }

    [Column("sector_id")]
    public int? SectorId { get; set; }

    [ForeignKey("SectorId")]
    public Sector? Sector { get; set; }

    [Column("ods_id")]
    public int? ODSId { get; set; }

    [ForeignKey("ODSId")]
    public ODS? ODS { get; set; }

    [MaxLength(100)]
    [Column("meta_resultado")]
    public string? MetaResultado { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
