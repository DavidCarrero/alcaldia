using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface ISubsecretariaService : IBaseService<SubsecretariaViewModel>
{
    Task<IEnumerable<SubsecretariaViewModel>> GetBySecretariaIdAsync(int secretariaId);
    Task<IEnumerable<SubsecretariaViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId);
    Task<Dictionary<string, int>> GetEstadisticasAsync();
    Task<IEnumerable<SubsecretariaViewModel>> SearchAsync(string searchTerm);
}
