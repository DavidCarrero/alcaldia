using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.DTOs;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [MaxLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
    public string Email { get; set; } = string.Empty;
}
