using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IPlanNacionalService
{
    Task<IEnumerable<PlanNacionalViewModel>> GetAllPlanesNacionalesAsync(bool incluirInactivas = false);
    Task<PlanNacionalViewModel?> GetPlanNacionalByIdAsync(int id);
    Task<PlanNacionalViewModel> CreatePlanNacionalAsync(PlanNacionalViewModel model);
    Task UpdatePlanNacionalAsync(int id, PlanNacionalViewModel model);
    Task DeletePlanNacionalAsync(int id);
    Task<IEnumerable<PlanNacionalViewModel>> SearchPlanesNacionalesAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
