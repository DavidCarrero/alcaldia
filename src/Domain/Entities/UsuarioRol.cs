using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_alcaldia.Domain.Entities;

[Table("usuarios_roles")]
public class UsuarioRol : BaseEntity
{
    [Column("id")]
    public new int Id { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; } = null!;

    [Column("rol_id")]
    public int RolId { get; set; }

    [ForeignKey("RolId")]
    public Rol Rol { get; set; } = null!;

    [Column("fecha_asignacion")]
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
}
