using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("alcaldes_vigencias")]
public class AlcaldeVigencia
{
    [Column("id")]
    public int Id { get; set; }

    [Column("alcalde_id")]
    public int AlcaldeId { get; set; }

    [ForeignKey("AlcaldeId")]
    public Alcalde Alcalde { get; set; } = null!;

    [Column("vigencia_id")]
    public int VigenciaId { get; set; }

    [ForeignKey("VigenciaId")]
    public Vigencia Vigencia { get; set; } = null!;

    [Column("fecha_asignacion")]
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    [Column("activo")]
    public bool Activo { get; set; } = true;
}
