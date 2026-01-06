using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("subsecretarias_responsables")]
public class SubsecretariaResponsable : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("subsecretaria_id")]
    public int SubsecretariaId { get; set; }

    [ForeignKey("SubsecretariaId")]
    public Subsecretaria Subsecretaria { get; set; } = null!;

    [Column("responsable_id")]
    public int ResponsableId { get; set; }

    [ForeignKey("ResponsableId")]
    public Responsable Responsable { get; set; } = null!;
}
