using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class IndicadorViewModel
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

    [StringLength(50)]
    [Display(Name = "Unidad de Medida")]
    public string? UnidadMedida { get; set; }

    [Display(Name = "Línea Base")]
    public decimal? LineaBase { get; set; }

    [Display(Name = "Meta Final")]
    public decimal? MetaFinal { get; set; }

    [Display(Name = "Responsable")]
    public int? ResponsableId { get; set; }

    [Display(Name = "Nombre Responsable")]
    public string? NombreResponsable { get; set; }

    [Display(Name = "Producto")]
    public int? ProductoId { get; set; }

    [Display(Name = "Código Producto")]
    public string? CodigoProducto { get; set; }

    [Display(Name = "Nombre Producto")]
    public string? NombreProducto { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
