using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IMetaODSService
{
    Task<IEnumerable<MetaODSViewModel>> GetAllMetasODSAsync(bool incluirInactivos = false);
    Task<MetaODSViewModel?> GetMetaODSByIdAsync(int id);
}
