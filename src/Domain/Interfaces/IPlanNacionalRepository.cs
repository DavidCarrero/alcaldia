using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IPlanNacionalRepository
{
    Task<IEnumerable<PlanNacional>> GetAllAsync(bool incluirInactivas = false);
    Task<PlanNacional?> GetByIdAsync(int id);
    Task<PlanNacional> CreateAsync(PlanNacional planNacional);
    Task UpdateAsync(PlanNacional planNacional);
    Task DeleteAsync(int id);
    Task<IEnumerable<PlanNacional>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
