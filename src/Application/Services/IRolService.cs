using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IRolService
{
    Task<IEnumerable<RolViewModel>> GetAllRolesAsync(bool incluirInactivos = false);
    Task<RolViewModel?> GetRolByIdAsync(int id);
    Task<RolViewModel> CreateRolAsync(RolViewModel model);
    Task UpdateRolAsync(int id, RolViewModel model);
    Task DeleteRolAsync(int id);
    Task<IEnumerable<RolViewModel>> SearchRolesAsync(string searchTerm);
    Task<bool> NameExistsAsync(string nombre, int? excludeRolId = null);
    Task<(int totalRoles, int permisosConfigurados, int usuariosAsignados, int rolesPersonalizados)> GetEstadisticasAsync();
}
