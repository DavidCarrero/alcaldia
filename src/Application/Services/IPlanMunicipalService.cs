using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IPlanMunicipalService
{
    Task<IEnumerable<PlanMunicipalViewModel>> GetAllPlanesMunicipalesAsync(bool incluirInactivas = false);
    Task<PlanMunicipalViewModel?> GetPlanMunicipalByIdAsync(int id);
    Task<PlanMunicipalViewModel> CreatePlanMunicipalAsync(PlanMunicipalViewModel model);
    Task UpdatePlanMunicipalAsync(int id, PlanMunicipalViewModel model);
    Task DeletePlanMunicipalAsync(int id);
    Task<IEnumerable<PlanMunicipalViewModel>> SearchPlanesMunicipalesAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
