using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Application.DTOs;

namespace Proyecto_alcaldia.Application.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioViewModel>> GetAllUsuariosAsync(bool incluirInactivos = false);
    Task<UsuarioViewModel?> GetUsuarioByIdAsync(int id);
    Task<UsuarioViewModel> CreateUsuarioAsync(CrearUsuarioViewModel model);
    Task UpdateUsuarioAsync(int id, EditarUsuarioViewModel model);
    Task DeleteUsuarioAsync(int id);
    Task<IEnumerable<UsuarioViewModel>> SearchUsuariosAsync(string searchTerm);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
    Task<(int activos, int inactivos, int sesionesActivas, int accesosHoy)> GetEstadisticasAsync();
    Task<UsuarioDto?> GetUsuarioByEmailAsync(string email);
    Task<bool> CambiarPasswordAsync(int usuarioId, string passwordActual, string nuevaPassword);
    Task CambiarTemaColorAsync(int usuarioId, string temaColor);
    Task ActualizarPerfilAsync(int usuarioId, UsuarioDto dto);
    Task ActualizarFotoPerfilAsync(int usuarioId, byte[] fotoBytes);
}
