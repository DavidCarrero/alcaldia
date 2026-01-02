using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IAlcaldeService : IBaseService<AlcaldeViewModel>
{
    Task<Dictionary<string, int>> GetEstadisticasAsync();
}
