using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class EvidenciaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Municipio")]
    public string? MunicipioAlcaldia { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Actividad")]
    public int? ActividadId { get; set; }

    [Display(Name = "Código Actividad")]
    public string? CodigoActividad { get; set; }

    [Display(Name = "Nombre Actividad")]
    public string? NombreActividad { get; set; }

    [StringLength(5)]
    [Display(Name = "Tipo Evidencia")]
    public string? TipoEvidencia { get; set; }

    [StringLength(255)]
    [Display(Name = "Nombre Archivo")]
    public string? NombreArchivo { get; set; }

    [StringLength(100)]
    [Display(Name = "Tipo MIME")]
    public string? TipoMime { get; set; }

    [Display(Name = "Tamaño (bytes)")]
    public long? TamanoBytes { get; set; }

    [Display(Name = "Ruta Archivo")]
    public string? RutaArchivo { get; set; }

    [Display(Name = "Fecha Captura")]
    [DataType(DataType.Date)]
    public DateTime? FechaCaptura { get; set; }

    [StringLength(200)]
    [Display(Name = "Tomado Por")]
    public string? TomadoPor { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
