using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class MunicipioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(10, ErrorMessage = "El código no puede exceder 10 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione al menos un departamento")]
    [Display(Name = "Departamentos")]
    public List<int> DepartamentoIds { get; set; } = new();

    // Para compatibilidad con el código existente
    [Display(Name = "Departamento")]
    public int DepartamentoId 
    { 
        get => DepartamentoIds.FirstOrDefault(); 
        set { if (value > 0 && !DepartamentoIds.Contains(value)) DepartamentoIds.Add(value); }
    }

    [Display(Name = "Departamentos")]
    public List<string> Departamentos { get; set; } = new();

    [Display(Name = "Código Departamento")]
    public string? CodigoDepartamento { get; set; }

    [Display(Name = "Nombre Departamento")]
    public string? NombreDepartamento { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
