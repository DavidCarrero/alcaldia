using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("departamentos")]
public class Departamento : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // Relaciones muchos-a-muchos con Municipios
    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
    
    // Otras relaciones
    public ICollection<Alcaldia> Alcaldias { get; set; } = new List<Alcaldia>();
}
