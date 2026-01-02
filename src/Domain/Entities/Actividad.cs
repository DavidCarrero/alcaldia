using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("actividades")]
public class Actividad : BaseEntity
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

    [Column("proyecto_id")]
    public int ProyectoId { get; set; }

    [ForeignKey("ProyectoId")]
    public Proyecto Proyecto { get; set; } = null!;

    [Column("responsable_id")]
    public int? ResponsableId { get; set; }

    [ForeignKey("ResponsableId")]
    public Responsable? Responsable { get; set; }

    [Column("vigencia_id")]
    public int? VigenciaId { get; set; }

    [ForeignKey("VigenciaId")]
    public Vigencia? Vigencia { get; set; }

    [MaxLength(5)]
    [Column("tipo_actividad")]
    public string? TipoActividad { get; set; }

    [Column("fecha_inicio_programada")]
    public DateTime? FechaInicioProgramada { get; set; }

    [Column("fecha_fin_programada")]
    public DateTime? FechaFinProgramada { get; set; }

    [Column("fecha_inicio_real")]
    public DateTime? FechaInicioReal { get; set; }

    [Column("fecha_fin_real")]
    public DateTime? FechaFinReal { get; set; }

    [Column("presupuesto_asignado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoAsignado { get; set; }

    [Column("presupuesto_ejecutado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoEjecutado { get; set; }

    [MaxLength(5)]
    [Column("unidad_medida")]
    public string? UnidadMedida { get; set; }

    [Column("meta_planeada", TypeName = "numeric(18,2)")]
    public decimal? MetaPlaneada { get; set; }

    [MaxLength(5)]
    [Column("estado_actividad")]
    public string? EstadoActividad { get; set; }

    [Column("porcentaje_avance", TypeName = "numeric(5,2)")]
    public decimal? PorcentajeAvance { get; set; }

    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
}
