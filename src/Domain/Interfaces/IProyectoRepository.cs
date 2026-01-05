using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IProyectoRepository
{
    Task<IEnumerable<Proyecto>> GetAllAsync(bool incluirInactivas = false);
    Task<Proyecto?> GetByIdAsync(int id);
    Task<Proyecto> CreateAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task DeleteAsync(int id);
    Task<IEnumerable<Proyecto>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
