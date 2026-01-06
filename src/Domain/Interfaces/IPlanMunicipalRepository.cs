using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IPlanMunicipalRepository
{
    Task<IEnumerable<PlanMunicipal>> GetAllAsync(bool incluirInactivas = false);
    Task<PlanMunicipal?> GetByIdAsync(int id);
    Task<PlanMunicipal> CreateAsync(PlanMunicipal planMunicipal);
    Task UpdateAsync(PlanMunicipal planMunicipal);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<PlanMunicipal>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
