using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("indicadores")]
public class Indicador : BaseEntity
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

    [MaxLength(50)]
    [Column("unidad_medida")]
    public string? UnidadMedida { get; set; }

    [Column("linea_base", TypeName = "numeric(18,2)")]
    public decimal? LineaBase { get; set; }

    [Column("meta_final", TypeName = "numeric(18,2)")]
    public decimal? MetaFinal { get; set; }

    [Column("responsable_id")]
    public int? ResponsableId { get; set; }

    [ForeignKey("ResponsableId")]
    public Responsable? Responsable { get; set; }

    [Column("producto_id")]
    public int? ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public Producto? Producto { get; set; }

    public ICollection<ProyectoIndicador> ProyectosIndicadores { get; set; } = new List<ProyectoIndicador>();
}
