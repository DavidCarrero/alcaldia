using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IPlanDepartamentalRepository
{
    Task<IEnumerable<PlanDepartamental>> GetAllAsync(bool incluirInactivas = false);
    Task<PlanDepartamental?> GetByIdAsync(int id);
    Task<PlanDepartamental> CreateAsync(PlanDepartamental planDepartamental);
    Task UpdateAsync(PlanDepartamental planDepartamental);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<PlanDepartamental>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
