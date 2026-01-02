using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("vigencias")]
public class Vigencia : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("alcaldia_id")]
    public int AlcaldiaId { get; set; }

    [ForeignKey("AlcaldiaId")]
    public Alcaldia Alcaldia { get; set; } = null!;

    [MaxLength(20)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("año")]
    public int Año { get; set; }

    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    [Column("activo")]
    public new bool Activo { get; set; } = true;

    public ICollection<AlcaldeVigencia> AlcaldesVigencias { get; set; } = new List<AlcaldeVigencia>();
}
