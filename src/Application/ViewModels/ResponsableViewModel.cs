using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class ResponsableViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es requerida")]
    [Display(Name = "Alcaldía")]
    public int AlcaldiaId { get; set; }

    [Display(Name = "Tipo de Documento")]
    [StringLength(5)]
    public string? TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de identificación es requerido")]
    [Display(Name = "Número de Identificación")]
    [StringLength(50)]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [Display(Name = "Nombre Completo")]
    [StringLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Display(Name = "Teléfono")]
    [StringLength(50)]
    public string? Telefono { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(200)]
    public string? Email { get; set; }

    [Display(Name = "Cargo")]
    [StringLength(100)]
    public string? Cargo { get; set; }

    [Display(Name = "Tipo de Responsable")]
    [StringLength(5)]
    public string? TipoResponsable { get; set; }

    [Display(Name = "Secretaría")]
    public int? SecretariaId { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Campos de solo lectura para mostrar
    public string? NitAlcaldia { get; set; }
    public string? MunicipioAlcaldia { get; set; }
    public string? CodigoSecretaria { get; set; }
    public string? NombreSecretaria { get; set; }
}
