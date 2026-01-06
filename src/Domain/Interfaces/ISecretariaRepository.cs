using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface ISecretariaRepository
{
    Task<IEnumerable<Secretaria>> GetAllAsync(bool incluirInactivas = false);
    Task<Secretaria?> GetByIdAsync(int id);
    Task<Secretaria> CreateAsync(Secretaria secretaria);
    Task UpdateAsync(Secretaria secretaria);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<Secretaria>> SearchAsync(string searchTerm);
    Task<IEnumerable<Secretaria>> GetByAlcaldiaIdAsync(int alcaldiaId);
}
