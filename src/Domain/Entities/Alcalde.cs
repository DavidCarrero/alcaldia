using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("alcaldes")]
public class Alcalde : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("alcaldia_id")]
    public int AlcaldiaId { get; set; }

    [ForeignKey("AlcaldiaId")]
    public Alcaldia Alcaldia { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    [Column("nombre_completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("tipo_documento")]
    public string? TipoDocumento { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("numero_documento")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Column("periodo_inicio")]
    public DateTime? PeriodoInicio { get; set; }

    [Column("periodo_fin")]
    public DateTime? PeriodoFin { get; set; }

    [MaxLength(200)]
    [Column("slogan")]
    public string? Slogan { get; set; }

    [MaxLength(100)]
    [Column("partido_politico")]
    public string? PartidoPolitico { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    [Column("correo_electronico")]
    public string? CorreoElectronico { get; set; }

    [Column("activo")]
    public new bool Activo { get; set; } = true;

    public ICollection<AlcaldeVigencia> AlcaldesVigencias { get; set; } = new List<AlcaldeVigencia>();
}
