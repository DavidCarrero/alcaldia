using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IVigenciaRepository
{
    Task<IEnumerable<Vigencia>> GetAllAsync(bool incluirInactivas = false);
    Task<Vigencia?> GetByIdAsync(int id);
    Task<Vigencia> CreateAsync(Vigencia vigencia);
    Task UpdateAsync(Vigencia vigencia);
    Task DeleteAsync(int id);
    Task<IEnumerable<Vigencia>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
