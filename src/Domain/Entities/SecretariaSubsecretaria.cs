using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("secretarias_subsecretarias")]
public class SecretariaSubsecretaria : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("secretaria_id")]
    public int SecretariaId { get; set; }

    [ForeignKey("SecretariaId")]
    public Secretaria Secretaria { get; set; } = null!;

    [Column("subsecretaria_id")]
    public int SubsecretariaId { get; set; }

    [ForeignKey("SubsecretariaId")]
    public Subsecretaria Subsecretaria { get; set; } = null!;
}
