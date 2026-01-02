using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class VigenciaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Municipio")]
    public string? MunicipioAlcaldia { get; set; }

    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    [Display(Name = "Código")]
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
    [Display(Name = "Año")]
    public int Año { get; set; }

    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
