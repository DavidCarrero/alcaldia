using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IProgramaRepository
{
    Task<IEnumerable<Programa>> GetAllAsync(bool incluirInactivas = false);
    Task<Programa?> GetByIdAsync(int id);
    Task<Programa> CreateAsync(Programa programa);
    Task UpdateAsync(Programa programa);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<Programa>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
