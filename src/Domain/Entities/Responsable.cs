using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("responsables")]
public class Responsable : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("alcaldia_id")]
    public int AlcaldiaId { get; set; }

    [ForeignKey("AlcaldiaId")]
    public Alcaldia Alcaldia { get; set; } = null!;

    [MaxLength(5)]
    [Column("tipo_documento")]
    public string? TipoDocumento { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("numero_identificacion")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("nombre_completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [MaxLength(200)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(100)]
    [Column("cargo")]
    public string? Cargo { get; set; }

    [MaxLength(5)]
    [Column("tipo_responsable")]
    public string? TipoResponsable { get; set; }

    [Column("secretaria_id")]
    public int? SecretariaId { get; set; }

    [ForeignKey("SecretariaId")]
    public Secretaria? Secretaria { get; set; }
}
