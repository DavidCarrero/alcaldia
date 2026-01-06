using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("ods_metas_ods")]
public class ODSMetaODS : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("ods_id")]
    public int ODSId { get; set; }

    [ForeignKey("ODSId")]
    public ODS ODS { get; set; } = null!;

    [Column("meta_ods_id")]
    public int MetaODSId { get; set; }

    [ForeignKey("MetaODSId")]
    public MetaODS MetaODS { get; set; } = null!;

    [Column("fecha_asociacion")]
    public DateTime FechaAsociacion { get; set; } = DateTime.UtcNow;
}
