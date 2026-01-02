using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IODSService
{
    Task<IEnumerable<ODSViewModel>> GetAllODSAsync(bool incluirInactivos = false);
    Task<ODSViewModel?> GetODSByIdAsync(int id);
    Task<ODSViewModel> CreateODSAsync(ODSViewModel model);
    Task UpdateODSAsync(int id, ODSViewModel model);
    Task DeleteODSAsync(int id);
    Task<IEnumerable<ODSViewModel>> SearchODSAsync(string searchTerm);
}
