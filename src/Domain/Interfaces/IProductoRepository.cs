using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetAllAsync(bool incluirInactivas = false);
    Task<Producto?> GetByIdAsync(int id);
    Task<Producto> CreateAsync(Producto producto);
    Task UpdateAsync(Producto producto);
    Task DeleteAsync(int id);
    Task<IEnumerable<Producto>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
