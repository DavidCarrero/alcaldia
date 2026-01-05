using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IIndicadorRepository
{
    Task<IEnumerable<Indicador>> GetAllAsync(bool incluirInactivas = false);
    Task<Indicador?> GetByIdAsync(int id);
    Task<Indicador> CreateAsync(Indicador indicador);
    Task UpdateAsync(Indicador indicador);
    Task DeleteAsync(int id);
    Task<IEnumerable<Indicador>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
