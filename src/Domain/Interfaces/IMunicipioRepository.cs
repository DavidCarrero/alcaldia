using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IMunicipioRepository
{
    Task<IEnumerable<Municipio>> GetAllAsync(bool incluirInactivos = false);
    Task<Municipio?> GetByIdAsync(int id);
    Task<Municipio> CreateAsync(Municipio municipio, List<int>? departamentoIds = null);
    Task UpdateAsync(Municipio municipio, List<int>? departamentoIds = null);
    Task DeleteAsync(int id, string deletedBy);
    Task<IEnumerable<Municipio>> SearchAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<IEnumerable<Municipio>> GetByDepartamentoIdAsync(int departamentoId);
}
