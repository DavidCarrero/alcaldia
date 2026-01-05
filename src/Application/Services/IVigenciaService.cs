using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IVigenciaService
{
    Task<IEnumerable<VigenciaViewModel>> GetAllVigenciasAsync(bool incluirInactivas = false);
    Task<VigenciaViewModel?> GetVigenciaByIdAsync(int id);
    Task<VigenciaViewModel> CreateVigenciaAsync(VigenciaViewModel model);
    Task UpdateVigenciaAsync(int id, VigenciaViewModel model);
    Task DeleteVigenciaAsync(int id);
    Task<IEnumerable<VigenciaViewModel>> SearchVigenciasAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
