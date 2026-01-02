using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("sectores")]
public class Sector : BaseEntity
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

    [Column("linea_estrategica_id")]
    public int LineaEstrategicaId { get; set; }

    [ForeignKey("LineaEstrategicaId")]
    public LineaEstrategica LineaEstrategica { get; set; } = null!;

    [Column("aplicacion", TypeName = "text")]
    public string? Aplicacion { get; set; }

    [Column("presupuesto_asignado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoAsignado { get; set; }

    public ICollection<Programa> Programas { get; set; } = new List<Programa>();
    public ICollection<PlanNacional> PlanesNacionales { get; set; } = new List<PlanNacional>();
    public ICollection<PlanDepartamental> PlanesDepartamentales { get; set; } = new List<PlanDepartamental>();
}
