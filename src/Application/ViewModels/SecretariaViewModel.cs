using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class SecretariaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [StringLength(20, ErrorMessage = "La sigla no puede exceder 20 caracteres")]
    [Display(Name = "Sigla")]
    public string? Sigla { get; set; }

    [StringLength(200, ErrorMessage = "El nombre del secretario no puede exceder 200 caracteres")]
    [Display(Name = "Secretario")]
    public string? Secretario { get; set; }

    [StringLength(200, ErrorMessage = "El correo no puede exceder 200 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
    [Display(Name = "Correo Institucional")]
    public string? CorreoInstitucional { get; set; }

    [StringLength(50, ErrorMessage = "El teléfono no puede exceder 50 caracteres")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [Display(Name = "Presupuesto Asignado")]
    public decimal? PresupuestoAsignado { get; set; }

    [Required(ErrorMessage = "La alcaldía es obligatoria")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "NIT Alcaldía")]
    public string? NitAlcaldia { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Subsecretarías")]
    public int CantidadSubsecretarias { get; set; }
}
