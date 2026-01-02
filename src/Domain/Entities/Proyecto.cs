using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("proyectos")]
public class Proyecto : BaseEntity
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

    [Column("codigo_bpin", TypeName = "numeric(18,0)")]
    public decimal? CodigoBPIN { get; set; }

    [Column("presupuesto_asignado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoAsignado { get; set; }

    [Column("presupuesto_ejecutado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoEjecutado { get; set; }

    [Column("valor_proyecto", TypeName = "numeric(18,2)")]
    public decimal? ValorProyecto { get; set; }

    [Column("responsable_id")]
    public int? ResponsableId { get; set; }

    [ForeignKey("ResponsableId")]
    public Responsable? Responsable { get; set; }

    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    [Column("estado_proyecto")]
    public bool? EstadoProyecto { get; set; }

    [Column("porcentaje_avance", TypeName = "numeric(5,2)")]
    public decimal? PorcentajeAvance { get; set; }

    public ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
    public ICollection<ProyectoIndicador> ProyectosIndicadores { get; set; } = new List<ProyectoIndicador>();
}
