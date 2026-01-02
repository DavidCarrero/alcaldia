using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class UsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
    [Display(Name = "Correo Electrónico")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string? NombreUsuario { get; set; }

    [Display(Name = "Roles Asignados")]
    public List<int> RolesSeleccionados { get; set; } = new List<int>();

    [Display(Name = "Roles")]
    public List<int> RolesIds { get; set; } = new List<int>();

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Último Acceso")]
    public DateTime? UltimoAcceso { get; set; }
}

public class CrearUsuarioViewModel : UsuarioViewModel
{
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}

public class EditarUsuarioViewModel : UsuarioViewModel
{
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña (dejar en blanco para no cambiar)")]
    public string? NuevaContrasena { get; set; }

    [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    public string? ConfirmarNuevaContrasena { get; set; }
}
