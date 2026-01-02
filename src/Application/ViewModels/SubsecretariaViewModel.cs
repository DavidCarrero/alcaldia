using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class SubsecretariaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es requerida")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [Display(Name = "Código")]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La secretaría es requerida")]
    [Display(Name = "Secretaría")]
    public int SecretariaId { get; set; }

    [Display(Name = "Responsable")]
    [StringLength(200)]
    public string? Responsable { get; set; }

    [Display(Name = "Correo Institucional")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [StringLength(200)]
    public string? CorreoInstitucional { get; set; }

    [Display(Name = "Teléfono")]
    [StringLength(50)]
    public string? Telefono { get; set; }

    [Display(Name = "Presupuesto Asignado")]
    public decimal? PresupuestoAsignado { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Campos de solo lectura para mostrar
    public string? NitAlcaldia { get; set; }
    public string? MunicipioAlcaldia { get; set; }
    public string? CodigoSecretaria { get; set; }
    public string? NombreSecretaria { get; set; }
}
