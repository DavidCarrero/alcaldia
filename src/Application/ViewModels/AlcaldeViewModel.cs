using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class AlcaldeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La alcaldía es requerida")]
    public int AlcaldiaId { get; set; }

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [Display(Name = "Nombre Completo")]
    [StringLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Display(Name = "Tipo de Documento")]
    [StringLength(50)]
    public string? TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es requerido")]
    [Display(Name = "Número de Documento")]
    [StringLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Display(Name = "Periodo Inicio")]
    public DateTime? PeriodoInicio { get; set; }

    [Display(Name = "Periodo Fin")]
    public DateTime? PeriodoFin { get; set; }

    [Display(Name = "Slogan")]
    [StringLength(200)]
    public string? Slogan { get; set; }

    [Display(Name = "Partido Político")]
    [StringLength(100)]
    public string? PartidoPolitico { get; set; }

    [Display(Name = "Correo Electrónico")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100)]
    public string? CorreoElectronico { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Para mostrar información relacionada
    public string? NitAlcaldia { get; set; }
    public string? MunicipioAlcaldia { get; set; }
}
