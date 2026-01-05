using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IEvidenciaRepository
{
    Task<IEnumerable<Evidencia>> GetAllAsync(bool incluirInactivas = false);
    Task<Evidencia?> GetByIdAsync(int id);
    Task<Evidencia> CreateAsync(Evidencia evidencia);
    Task UpdateAsync(Evidencia evidencia);
    Task DeleteAsync(int id);
    Task<IEnumerable<Evidencia>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<int> CountActiveAsync();
}
