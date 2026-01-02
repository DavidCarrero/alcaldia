using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface ISubsecretariaRepository : IBaseRepository<Subsecretaria>
{
    Task<IEnumerable<Subsecretaria>> GetBySecretariaIdAsync(int secretariaId);
    Task<IEnumerable<Subsecretaria>> GetByAlcaldiaIdAsync(int alcaldiaId);
    Task<int> GetTotalActivasAsync();
}
