using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class ProyectoIndicadorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Municipio")]
    public string? MunicipioAlcaldia { get; set; }

    [Required(ErrorMessage = "El proyecto es obligatorio")]
    [Display(Name = "Proyecto")]
    public int ProyectoId { get; set; }

    [Display(Name = "Código Proyecto")]
    public string? CodigoProyecto { get; set; }

    [Display(Name = "Nombre Proyecto")]
    public string? NombreProyecto { get; set; }

    [Required(ErrorMessage = "El indicador es obligatorio")]
    [Display(Name = "Indicador")]
    public int IndicadorId { get; set; }

    [Display(Name = "Código Indicador")]
    public string? CodigoIndicador { get; set; }

    [Display(Name = "Nombre Indicador")]
    public string? NombreIndicador { get; set; }

    [Display(Name = "Porcentaje Avance")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
    public decimal? PorcentajeAvance { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
