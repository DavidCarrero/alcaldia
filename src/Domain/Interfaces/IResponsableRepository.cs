using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IResponsableRepository : IBaseRepository<Responsable>
{
    Task<IEnumerable<Responsable>> GetBySecretariaIdAsync(int secretariaId);
    Task<IEnumerable<Responsable>> GetByAlcaldiaIdAsync(int alcaldiaId);
    Task<int> GetTotalActivosAsync();
}
