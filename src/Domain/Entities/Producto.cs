using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("productos")]
public class Producto : BaseEntity
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

    [Column("programa_id")]
    public int ProgramaId { get; set; }

    [ForeignKey("ProgramaId")]
    public Programa Programa { get; set; } = null!;

    [Column("presupuesto_asignado", TypeName = "numeric(18,2)")]
    public decimal? PresupuestoAsignado { get; set; }

    public ICollection<Indicador> Indicadores { get; set; } = new List<Indicador>();
}
