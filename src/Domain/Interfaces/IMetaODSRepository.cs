using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IMetaODSRepository
{
    Task<IEnumerable<MetaODS>> GetAllAsync(bool incluirInactivas = false);
    Task<MetaODS?> GetByIdAsync(int id);
    Task<MetaODS> CreateAsync(MetaODS metaODS);
    Task UpdateAsync(MetaODS metaODS);
    Task DeleteAsync(int id);
    Task<IEnumerable<MetaODS>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null);
    Task<int> CountActiveAsync();
}
