using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IMetaODSService
{
    Task<IEnumerable<MetaODSViewModel>> GetAllMetasODSAsync(bool incluirInactivas = false);
    Task<MetaODSViewModel?> GetMetaODSByIdAsync(int id);
    Task<MetaODSViewModel> CreateMetaODSAsync(MetaODSViewModel model);
    Task UpdateMetaODSAsync(int id, MetaODSViewModel model);
    Task DeleteMetaODSAsync(int id);
    Task<IEnumerable<MetaODSViewModel>> SearchMetasODSAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null);
}
