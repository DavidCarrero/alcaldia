using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class PlanMunicipalViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Municipio Alcaldía")]
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

    [Display(Name = "Municipio")]
    public int? MunicipioId { get; set; }

    [Display(Name = "Código Municipio")]
    public string? CodigoMunicipio { get; set; }

    [Display(Name = "Nombre Municipio")]
    public string? NombreMunicipio { get; set; }

    [Display(Name = "Alcalde")]
    public int? AlcaldeId { get; set; }

    [Display(Name = "Nombre Alcalde")]
    public string? NombreAlcalde { get; set; }

    [Display(Name = "Plan Nacional")]
    public int? PlanNacionalId { get; set; }

    [Display(Name = "Código Plan Nacional")]
    public string? CodigoPlanNacional { get; set; }

    [Display(Name = "Nombre Plan Nacional")]
    public string? NombrePlanNacional { get; set; }

    [Display(Name = "Plan Departamental")]
    public int? PlanDptlId { get; set; }

    [Display(Name = "Código Plan Departamental")]
    public string? CodigoPlanDepartamental { get; set; }

    [Display(Name = "Nombre Plan Departamental")]
    public string? NombrePlanDepartamental { get; set; }

    [Display(Name = "Fecha Inicio")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha Fin")]
    [DataType(DataType.Date)]
    public DateTime? FechaFin { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Programas")]
    public int CantidadProgramas { get; set; }
}
