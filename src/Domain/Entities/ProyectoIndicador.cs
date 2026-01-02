using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("proyectos_indicadores")]
public class ProyectoIndicador : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("alcaldia_id")]
    public int AlcaldiaId { get; set; }

    [ForeignKey("AlcaldiaId")]
    public Alcaldia Alcaldia { get; set; } = null!;

    [Column("proyecto_id")]
    public int ProyectoId { get; set; }

    [ForeignKey("ProyectoId")]
    public Proyecto Proyecto { get; set; } = null!;

    [Column("indicador_id")]
    public int IndicadorId { get; set; }

    [ForeignKey("IndicadorId")]
    public Indicador Indicador { get; set; } = null!;

    [Column("porcentaje_avance", TypeName = "numeric(5,2)")]
    public decimal? PorcentajeAvance { get; set; }
}
