using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IAlcaldeRepository : IBaseRepository<Alcalde>
{
    Task<int> GetTotalActivosAsync();
    Task<int> GetTotalEnPeriodoAsync();
    Task<int> GetTotalPartidosAsync();
}
