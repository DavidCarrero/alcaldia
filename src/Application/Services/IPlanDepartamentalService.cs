using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IPlanDepartamentalService
{
    Task<IEnumerable<PlanDepartamentalViewModel>> GetAllPlanesDepartamentalesAsync(bool incluirInactivas = false);
    Task<PlanDepartamentalViewModel?> GetPlanDepartamentalByIdAsync(int id);
    Task<PlanDepartamentalViewModel> CreatePlanDepartamentalAsync(PlanDepartamentalViewModel model);
    Task UpdatePlanDepartamentalAsync(int id, PlanDepartamentalViewModel model);
    Task DeletePlanDepartamentalAsync(int id);
    Task<IEnumerable<PlanDepartamentalViewModel>> SearchPlanesDepartamentalesAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
