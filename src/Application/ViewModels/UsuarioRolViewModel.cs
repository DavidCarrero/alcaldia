using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class UsuarioRolViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio")]
    [Display(Name = "Usuario")]
    public int UsuarioId { get; set; }

    [Display(Name = "Nombre Usuario")]
    public string? NombreUsuario { get; set; }

    [Display(Name = "Correo Usuario")]
    public string? CorreoUsuario { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }

    [Display(Name = "Nombre Rol")]
    public string? NombreRol { get; set; }

    [Display(Name = "Fecha Asignación")]
    [DataType(DataType.Date)]
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
