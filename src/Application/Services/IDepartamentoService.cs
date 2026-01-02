using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IDepartamentoService
{
    Task<IEnumerable<DepartamentoViewModel>> GetAllDepartamentosAsync(bool incluirInactivos = false);
    Task<DepartamentoViewModel?> GetDepartamentoByIdAsync(int id);
    Task<DepartamentoViewModel> CreateDepartamentoAsync(DepartamentoViewModel model);
    Task UpdateDepartamentoAsync(int id, DepartamentoViewModel model);
    Task DeleteDepartamentoAsync(int id);
    Task<IEnumerable<DepartamentoViewModel>> SearchDepartamentosAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
