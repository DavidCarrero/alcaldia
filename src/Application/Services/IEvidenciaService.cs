using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IEvidenciaService
{
    Task<IEnumerable<EvidenciaViewModel>> GetAllEvidenciasAsync(bool incluirInactivas = false);
    Task<EvidenciaViewModel?> GetEvidenciaByIdAsync(int id);
    Task<EvidenciaViewModel> CreateEvidenciaAsync(EvidenciaViewModel model);
    Task UpdateEvidenciaAsync(int id, EvidenciaViewModel model);
    Task DeleteEvidenciaAsync(int id);
    Task<IEnumerable<EvidenciaViewModel>> SearchEvidenciasAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
