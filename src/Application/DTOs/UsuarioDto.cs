namespace Proyecto_alcaldia.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string? NombreUsuario { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
    public string? TemaColor { get; set; }
    public List<string> Roles { get; set; } = new();
}
