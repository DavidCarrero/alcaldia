using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface ISectorRepository
{
    Task<IEnumerable<Sector>> GetAllAsync(bool incluirInactivas = false);
    Task<Sector?> GetByIdAsync(int id);
    Task<Sector> CreateAsync(Sector sector);
    Task UpdateAsync(Sector sector);
    Task DeleteAsync(int id);
    Task<IEnumerable<Sector>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
