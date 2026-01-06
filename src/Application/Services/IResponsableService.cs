using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IResponsableService : IBaseService<ResponsableViewModel>
{
    Task<IEnumerable<ResponsableViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId);
    Task<Dictionary<string, int>> GetEstadisticasAsync();
    Task<IEnumerable<ResponsableViewModel>> SearchAsync(string searchTerm);
}
