using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface ILineaEstrategicaService
{
    Task<IEnumerable<LineaEstrategicaViewModel>> GetAllLineasEstrategicasAsync(bool incluirInactivas = false);
    Task<LineaEstrategicaViewModel?> GetLineaEstrategicaByIdAsync(int id);
    Task<LineaEstrategicaViewModel> CreateLineaEstrategicaAsync(LineaEstrategicaViewModel model);
    Task UpdateLineaEstrategicaAsync(int id, LineaEstrategicaViewModel model);
    Task DeleteLineaEstrategicaAsync(int id);
    Task<IEnumerable<LineaEstrategicaViewModel>> SearchLineasEstrategicasAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
