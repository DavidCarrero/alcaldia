using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("evidencias")]
public class Evidencia : BaseEntity
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

    [Column("actividad_id")]
    public int ActividadId { get; set; }

    [ForeignKey("ActividadId")]
    public Actividad Actividad { get; set; } = null!;

    [MaxLength(5)]
    [Column("tipo_evidencia")]
    public string? TipoEvidencia { get; set; }

    [MaxLength(255)]
    [Column("nombre_archivo")]
    public string? NombreArchivo { get; set; }

    [MaxLength(100)]
    [Column("tipo_mime")]
    public string? TipoMime { get; set; }

    [Column("tamano_bytes")]
    public long? TamanoBytes { get; set; }

    [MaxLength(255)]
    [Column("hash_archivo")]
    public string? HashArchivo { get; set; }

    [Column("ruta_almacenamiento", TypeName = "text")]
    public string? RutaAlmacenamiento { get; set; }

    [Column("url_publica", TypeName = "text")]
    public string? URLPublica { get; set; }

    [Column("fecha_evidencia")]
    public DateTime? FechaEvidencia { get; set; }

    [Column("ubicacion_captura", TypeName = "text")]
    public string? UbicacionCaptura { get; set; }

    [Column("verificada")]
    public int? Verificada { get; set; }

    [Column("verificada_por")]
    public int? VerificadaPor { get; set; }

    [ForeignKey("VerificadaPor")]
    public Usuario? VerificadaPorUsuario { get; set; }
}
