using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IActividadRepository
{
    Task<IEnumerable<Actividad>> GetAllAsync(bool incluirInactivas = false);
    Task<Actividad?> GetByIdAsync(int id);
    Task<Actividad> CreateAsync(Actividad actividad);
    Task UpdateAsync(Actividad actividad);
    Task DeleteAsync(int id);
    Task<IEnumerable<Actividad>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
