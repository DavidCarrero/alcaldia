using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("ods")]
public class ODS : BaseEntity
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

    [Column("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    [MaxLength(20)]
    [Column("nivel_impacto")]
    public string? NivelImpacto { get; set; }

    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "ACTIVO";

    public ICollection<Programa> Programas { get; set; } = new List<Programa>();
    public ICollection<ODSMetaODS> ODSMetasODS { get; set; } = new List<ODSMetaODS>();
}
