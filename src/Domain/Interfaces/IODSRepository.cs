using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IODSRepository
{
    Task<IEnumerable<ODS>> GetAllAsync(bool incluirInactivos = false);
    Task<ODS?> GetByIdAsync(int id);
    Task<ODS> CreateAsync(ODS ods);
    Task UpdateAsync(ODS ods);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<ODS>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null);
    Task<int> CountActiveAsync();
}
