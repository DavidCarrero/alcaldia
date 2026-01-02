using System.ComponentModel.DataAnnotations;

namespace Proyecto_alcaldia.Application.ViewModels;

public class RolViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    [Display(Name = "Nombre del Rol")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Permisos")]
    public Dictionary<string, PermisosModulo> Permisos { get; set; } = new Dictionary<string, PermisosModulo>();

    [Display(Name = "Permisos por Módulo")]
    public Dictionary<string, PermisosModulo> PermisosModulo { get; set; } = new Dictionary<string, PermisosModulo>();
}

public class PermisosModulo
{
    public string NombreModulo { get; set; } = string.Empty;
    public bool Ver { get; set; }
    public bool Crear { get; set; }
    public bool Editar { get; set; }
    public bool Eliminar { get; set; }
}
