using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class ProductoViewModel
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

    [Display(Name = "Programa")]
    public int? ProgramaId { get; set; }

    [Display(Name = "Código Programa")]
    public string? CodigoPrograma { get; set; }

    [Display(Name = "Nombre Programa")]
    public string? NombrePrograma { get; set; }

    [Display(Name = "Presupuesto Asignado")]
    [DataType(DataType.Currency)]
    public decimal? PresupuestoAsignado { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Indicadores")]
    public int CantidadIndicadores { get; set; }
}
