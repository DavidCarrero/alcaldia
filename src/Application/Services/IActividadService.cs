using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IActividadService
{
    Task<IEnumerable<ActividadViewModel>> GetAllActividadesAsync(bool incluirInactivas = false);
    Task<ActividadViewModel?> GetActividadByIdAsync(int id);
    Task<ActividadViewModel> CreateActividadAsync(ActividadViewModel model);
    Task UpdateActividadAsync(int id, ActividadViewModel model);
    Task DeleteActividadAsync(int id);
    Task<IEnumerable<ActividadViewModel>> SearchActividadesAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
