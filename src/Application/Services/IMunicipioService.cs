using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IMunicipioService
{
    Task<IEnumerable<MunicipioViewModel>> GetAllMunicipiosAsync(bool incluirInactivos = false);
    Task<MunicipioViewModel?> GetMunicipioByIdAsync(int id);
    Task<MunicipioViewModel> CreateMunicipioAsync(MunicipioViewModel model);
    Task UpdateMunicipioAsync(int id, MunicipioViewModel model);
    Task DeleteMunicipioAsync(int id);
    Task<IEnumerable<MunicipioViewModel>> SearchMunicipiosAsync(string searchTerm);
    Task<IEnumerable<MunicipioViewModel>> GetByDepartamentoIdAsync(int departamentoId);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
