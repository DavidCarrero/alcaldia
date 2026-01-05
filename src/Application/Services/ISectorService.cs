using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface ISectorService
{
    Task<IEnumerable<SectorViewModel>> GetAllSectoresAsync(bool incluirInactivas = false);
    Task<SectorViewModel?> GetSectorByIdAsync(int id);
    Task<SectorViewModel> CreateSectorAsync(SectorViewModel model);
    Task UpdateSectorAsync(int id, SectorViewModel model);
    Task DeleteSectorAsync(int id);
    Task<IEnumerable<SectorViewModel>> SearchSectoresAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
