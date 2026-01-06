using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class DepartamentoService : IDepartamentoService
{
    private readonly IDepartamentoRepository _departamentoRepository;
    private readonly ILogger<DepartamentoService> _logger;

    public DepartamentoService(IDepartamentoRepository departamentoRepository, ILogger<DepartamentoService> logger)
    {
        _departamentoRepository = departamentoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<DepartamentoViewModel>> GetAllDepartamentosAsync(bool incluirInactivos = false)
    {
        var departamentos = await _departamentoRepository.GetAllAsync(incluirInactivos);
        return departamentos.Select(MapToViewModel);
    }

    public async Task<DepartamentoViewModel?> GetDepartamentoByIdAsync(int id)
    {
        var departamento = await _departamentoRepository.GetByIdAsync(id);
        return departamento != null ? MapToViewModel(departamento) : null;
    }

    public async Task<DepartamentoViewModel> CreateDepartamentoAsync(DepartamentoViewModel model)
    {
        if (await _departamentoRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un departamento con ese código");
        }

        var departamento = new Departamento
        {
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _departamentoRepository.CreateAsync(departamento);
        _logger.LogInformation($"Departamento creado: {departamento.Nombre}");

        return MapToViewModel(departamento);
    }

    public async Task UpdateDepartamentoAsync(int id, DepartamentoViewModel model)
    {
        var departamento = await _departamentoRepository.GetByIdAsync(id);
        if (departamento == null)
        {
            throw new InvalidOperationException("Departamento no encontrado");
        }

        if (await _departamentoRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un departamento con ese código");
        }

        departamento.Codigo = model.Codigo;
        departamento.Nombre = model.Nombre;
        departamento.Activo = model.Activo;
        departamento.FechaActualizacion = DateTime.UtcNow;

        await _departamentoRepository.UpdateAsync(departamento);
        _logger.LogInformation($"Departamento actualizado: {departamento.Nombre}");
    }

    public async Task DeleteDepartamentoAsync(int id)
    {
        await _departamentoRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Departamento desactivado: ID {id}");
    }

    public async Task<IEnumerable<DepartamentoViewModel>> SearchDepartamentosAsync(string searchTerm)
    {
        var departamentos = await _departamentoRepository.SearchAsync(searchTerm);
        return departamentos.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _departamentoRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static DepartamentoViewModel MapToViewModel(Departamento departamento)
    {
        return new DepartamentoViewModel
        {
            Id = departamento.Id,
            Codigo = departamento.Codigo,
            Nombre = departamento.Nombre,
            Activo = departamento.Activo,
            CantidadMunicipios = departamento.Municipios?.Count ?? 0
        };
    }
}
