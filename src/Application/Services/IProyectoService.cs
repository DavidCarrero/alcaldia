using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IProyectoService
{
    Task<IEnumerable<ProyectoViewModel>> GetAllProyectosAsync(bool incluirInactivas = false);
    Task<ProyectoViewModel?> GetProyectoByIdAsync(int id);
    Task<ProyectoViewModel> CreateProyectoAsync(ProyectoViewModel model);
    Task UpdateProyectoAsync(int id, ProyectoViewModel model);
    Task DeleteProyectoAsync(int id);
    Task<IEnumerable<ProyectoViewModel>> SearchProyectosAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
