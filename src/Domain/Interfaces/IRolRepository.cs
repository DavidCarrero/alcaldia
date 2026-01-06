using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IRolRepository
{
    Task<IEnumerable<Rol>> GetAllAsync(bool incluirInactivos = false);
    Task<Rol?> GetByIdAsync(int id);
    Task<Rol?> GetByNameAsync(string nombre);
    Task<Rol> CreateAsync(Rol rol);
    Task UpdateAsync(Rol rol);
    Task DeleteAsync(int id, string deletedBy);
    Task<bool> NameExistsAsync(string nombre, int? excludeRolId = null);
    Task<IEnumerable<Rol>> SearchAsync(string searchTerm);
    Task<int> CountActiveRolesAsync();
    Task<int> GetUsuariosCountByRolAsync(int rolId);
}
