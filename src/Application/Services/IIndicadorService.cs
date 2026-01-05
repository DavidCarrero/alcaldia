using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IIndicadorService
{
    Task<IEnumerable<IndicadorViewModel>> GetAllIndicadoresAsync(bool incluirInactivas = false);
    Task<IndicadorViewModel?> GetIndicadorByIdAsync(int id);
    Task<IndicadorViewModel> CreateIndicadorAsync(IndicadorViewModel model);
    Task UpdateIndicadorAsync(int id, IndicadorViewModel model);
    Task DeleteIndicadorAsync(int id);
    Task<IEnumerable<IndicadorViewModel>> SearchIndicadoresAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
