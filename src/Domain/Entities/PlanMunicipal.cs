using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("planes_municipales")]
public class PlanMunicipal : BaseEntity
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

    [Column("municipio_id")]
    public int? MunicipioId { get; set; }

    [ForeignKey("MunicipioId")]
    public Municipio? Municipio { get; set; }

    [Column("alcalde_id")]
    public int? AlcaldeId { get; set; }

    [ForeignKey("AlcaldeId")]
    public Alcalde? Alcalde { get; set; }

    [Column("plan_nacional_id")]
    public int? PlanNacionalId { get; set; }

    [ForeignKey("PlanNacionalId")]
    public PlanNacional? PlanNacional { get; set; }

    [Column("plan_dptl_id")]
    public int? PlanDptlId { get; set; }

    [ForeignKey("PlanDptlId")]
    public PlanDepartamental? PlanDepartamental { get; set; }

    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    public ICollection<Programa> Programas { get; set; } = new List<Programa>();
}
