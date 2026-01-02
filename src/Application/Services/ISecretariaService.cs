using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface ISecretariaService
{
    Task<IEnumerable<SecretariaViewModel>> GetAllSecretariasAsync(bool incluirInactivas = false);
    Task<SecretariaViewModel?> GetSecretariaByIdAsync(int id);
    Task<SecretariaViewModel> CreateSecretariaAsync(SecretariaViewModel model);
    Task UpdateSecretariaAsync(int id, SecretariaViewModel model);
    Task DeleteSecretariaAsync(int id);
    Task<IEnumerable<SecretariaViewModel>> SearchSecretariasAsync(string searchTerm);
    Task<IEnumerable<SecretariaViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId);
}
