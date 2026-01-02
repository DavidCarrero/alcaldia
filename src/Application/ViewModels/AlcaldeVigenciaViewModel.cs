using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class AlcaldeVigenciaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El alcalde es obligatorio")]
    [Display(Name = "Alcalde")]
    public int AlcaldeId { get; set; }

    [Display(Name = "Nombre Alcalde")]
    public string? NombreAlcalde { get; set; }

    [Display(Name = "Documento Alcalde")]
    public string? NumeroDocumentoAlcalde { get; set; }

    [Required(ErrorMessage = "La vigencia es obligatoria")]
    [Display(Name = "Vigencia")]
    public int VigenciaId { get; set; }

    [Display(Name = "Código Vigencia")]
    public string? CodigoVigencia { get; set; }

    [Display(Name = "Nombre Vigencia")]
    public string? NombreVigencia { get; set; }

    [Display(Name = "Año Vigencia")]
    public int? AñoVigencia { get; set; }

    [Display(Name = "Fecha Asignación")]
    [DataType(DataType.Date)]
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
