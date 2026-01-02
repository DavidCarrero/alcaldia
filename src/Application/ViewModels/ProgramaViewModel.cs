using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class ProgramaViewModel
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

    [Display(Name = "Presupuesto Cuatrienio")]
    [DataType(DataType.Currency)]
    public decimal? PresupuestoCuatrienio { get; set; }

    [Display(Name = "Plan Municipal")]
    public int? PlanMunicipalId { get; set; }

    [Display(Name = "Código Plan Municipal")]
    public string? CodigoPlanMunicipal { get; set; }

    [Display(Name = "Nombre Plan Municipal")]
    public string? NombrePlanMunicipal { get; set; }

    [Required(ErrorMessage = "El sector es obligatorio")]
    [Display(Name = "Sector")]
    public int SectorId { get; set; }

    [Display(Name = "Código Sector")]
    public string? CodigoSector { get; set; }

    [Display(Name = "Nombre Sector")]
    public string? NombreSector { get; set; }

    [Display(Name = "ODS")]
    public int? ODSId { get; set; }

    [Display(Name = "Código ODS")]
    public string? CodigoODS { get; set; }

    [Display(Name = "Nombre ODS")]
    public string? NombreODS { get; set; }

    [StringLength(100)]
    [Display(Name = "Meta Resultado")]
    public string? MetaResultado { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Productos")]
    public int CantidadProductos { get; set; }
}
