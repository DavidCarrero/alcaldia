using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface ILineaEstrategicaRepository
{
    Task<IEnumerable<LineaEstrategica>> GetAllAsync(bool incluirInactivas = false);
    Task<LineaEstrategica?> GetByIdAsync(int id);
    Task<LineaEstrategica> CreateAsync(LineaEstrategica lineaEstrategica);
    Task UpdateAsync(LineaEstrategica lineaEstrategica);
    Task DeleteAsync(int id);
    Task<IEnumerable<LineaEstrategica>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
