using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("roles")]
public class Rol : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("activo")]
    public new bool Activo { get; set; } = true;

    public ICollection<UsuarioRol> UsuariosRoles { get; set; } = new List<UsuarioRol>();
}
