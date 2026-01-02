using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("alcaldias")]
public class Alcaldia : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("nit")]
    public string Nit { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("logo")]
    public string? Logo { get; set; }

    [Column("municipio_id")]
    public int? MunicipioId { get; set; }

    [ForeignKey("MunicipioId")]
    public Municipio? Municipio { get; set; }

    [Column("departamento_id")]
    public int? DepartamentoId { get; set; }

    [ForeignKey("DepartamentoId")]
    public Departamento? Departamento { get; set; }

    [MaxLength(200)]
    [Column("direccion")]
    public string? Direccion { get; set; }

    [MaxLength(20)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    [Column("correo_institucional")]
    public string? CorreoInstitucional { get; set; }

    [Column("alcalde_actual_id")]
    public int? AlcaldeActualId { get; set; }

    [Column("activo")]
    public new bool Activo { get; set; } = true;

    public ICollection<Alcalde> Alcaldes { get; set; } = new List<Alcalde>();
    public ICollection<Vigencia> Vigencias { get; set; } = new List<Vigencia>();
    public ICollection<Secretaria> Secretarias { get; set; } = new List<Secretaria>();
    public ICollection<Subsecretaria> Subsecretarias { get; set; } = new List<Subsecretaria>();
    public ICollection<Responsable> Responsables { get; set; } = new List<Responsable>();
}
