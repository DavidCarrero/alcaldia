using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class AlcaldiaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El NIT es obligatorio")]
    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    [Display(Name = "NIT")]
    public string Nit { get; set; } = string.Empty;

    [Required(ErrorMessage = "El municipio es obligatorio")]
    [Display(Name = "Municipio")]
    public int MunicipioId { get; set; }

    [Display(Name = "Código Municipio")]
    public string? CodigoMunicipio { get; set; }

    [Display(Name = "Nombre Municipio")]
    public string? NombreMunicipio { get; set; }

    [Display(Name = "Departamento")]
    public string? NombreDepartamento { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Phone(ErrorMessage = "El teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
    [Display(Name = "Correo Institucional")]
    public string? CorreoInstitucional { get; set; }

    [Display(Name = "Logo Institucional")]
    public string? Logo { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
