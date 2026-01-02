using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("usuarios")]
public class Usuario : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre_completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    [Column("correo_electronico")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("contrasena_hash")]
    public string ContrasenaHash { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("nombre_usuario")]
    public string? NombreUsuario { get; set; }

    [Column("ultimo_acceso")]
    public DateTime? UltimoAcceso { get; set; }

    [Column("activo")]
    public new bool Activo { get; set; } = true;

    [MaxLength(20)]
    [Column("tema_color")]
    public string? TemaColor { get; set; } = "default";

    public ICollection<UsuarioRol> UsuariosRoles { get; set; } = new List<UsuarioRol>();
}
