using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IAlcaldiaService
{
    Task<IEnumerable<AlcaldiaViewModel>> GetAllAlcaldiasAsync(bool incluirInactivas = false);
    Task<AlcaldiaViewModel?> GetAlcaldiaByIdAsync(int id);
    Task<AlcaldiaViewModel> CreateAlcaldiaAsync(AlcaldiaViewModel model);
    Task UpdateAlcaldiaAsync(int id, AlcaldiaViewModel model);
    Task DeleteAlcaldiaAsync(int id);
    Task<IEnumerable<AlcaldiaViewModel>> SearchAlcaldiasAsync(string searchTerm);
    Task<bool> NitExistsAsync(string nit, int? excludeId = null);
}
