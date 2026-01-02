using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IAlcaldiaRepository
{
    Task<IEnumerable<Alcaldia>> GetAllAsync(bool incluirInactivas = false);
    Task<Alcaldia?> GetByIdAsync(int id);
    Task<Alcaldia> CreateAsync(Alcaldia alcaldia);
    Task UpdateAsync(Alcaldia alcaldia);
    Task DeleteAsync(int id);
    Task<IEnumerable<Alcaldia>> SearchAsync(string searchTerm);
    Task<bool> NitExistsAsync(string nit, int? excludeId = null);
    Task<int> CountActiveAsync();
}
