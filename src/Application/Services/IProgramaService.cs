using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IProgramaService
{
    Task<IEnumerable<ProgramaViewModel>> GetAllProgramasAsync(bool incluirInactivas = false);
    Task<ProgramaViewModel?> GetProgramaByIdAsync(int id);
    Task<ProgramaViewModel> CreateProgramaAsync(ProgramaViewModel model);
    Task UpdateProgramaAsync(int id, ProgramaViewModel model);
    Task DeleteProgramaAsync(int id);
    Task<IEnumerable<ProgramaViewModel>> SearchProgramasAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
