using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAllAsync(bool incluirInactivos = false);
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByUsernameAsync(string username);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(int id, string deletedBy);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
    Task<IEnumerable<Usuario>> SearchAsync(string searchTerm);
    Task<int> CountActiveUsersAsync();
    Task<int> CountInactiveUsersAsync();
}
