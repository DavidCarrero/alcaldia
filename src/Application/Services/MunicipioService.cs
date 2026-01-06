using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class MunicipioService : IMunicipioService
{
    private readonly IMunicipioRepository _municipioRepository;
    private readonly ILogger<MunicipioService> _logger;

    public MunicipioService(IMunicipioRepository municipioRepository, ILogger<MunicipioService> logger)
    {
        _municipioRepository = municipioRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MunicipioViewModel>> GetAllMunicipiosAsync(bool incluirInactivos = false)
    {
        var municipios = await _municipioRepository.GetAllAsync(incluirInactivos);
        return municipios.Select(MapToViewModel);
    }

    public async Task<MunicipioViewModel?> GetMunicipioByIdAsync(int id)
    {
        var municipio = await _municipioRepository.GetByIdAsync(id);
        return municipio != null ? MapToViewModel(municipio) : null;
    }

    public async Task<MunicipioViewModel> CreateMunicipioAsync(MunicipioViewModel model)
    {
        if (await _municipioRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un municipio con ese código");
        }

        var municipio = new Municipio
        {
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _municipioRepository.CreateAsync(municipio, model.DepartamentoIds);
        _logger.LogInformation($"Municipio creado: {municipio.Nombre}");

        return MapToViewModel(municipio);
    }

    public async Task UpdateMunicipioAsync(int id, MunicipioViewModel model)
    {
        var municipio = await _municipioRepository.GetByIdAsync(id);
        if (municipio == null)
        {
            throw new InvalidOperationException("Municipio no encontrado");
        }

        if (await _municipioRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un municipio con ese código");
        }

        municipio.Codigo = model.Codigo;
        municipio.Nombre = model.Nombre;
        municipio.Activo = model.Activo;
        municipio.FechaActualizacion = DateTime.UtcNow;

        await _municipioRepository.UpdateAsync(municipio, model.DepartamentoIds);
        _logger.LogInformation($"Municipio actualizado: {municipio.Nombre}");
    }

    public async Task DeleteMunicipioAsync(int id)
    {
        await _municipioRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Municipio desactivado: ID {id}");
    }

    public async Task<IEnumerable<MunicipioViewModel>> SearchMunicipiosAsync(string searchTerm)
    {
        var municipios = await _municipioRepository.SearchAsync(searchTerm);
        return municipios.Select(MapToViewModel);
    }

    public async Task<IEnumerable<MunicipioViewModel>> GetByDepartamentoIdAsync(int departamentoId)
    {
        var municipios = await _municipioRepository.GetByDepartamentoIdAsync(departamentoId);
        return municipios.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _municipioRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static MunicipioViewModel MapToViewModel(Municipio municipio)
    {
        var departamentos = municipio.Departamentos?.ToList() ?? new List<Departamento>();
        
        return new MunicipioViewModel
        {
            Id = municipio.Id,
            Codigo = municipio.Codigo,
            Nombre = municipio.Nombre,
            DepartamentoIds = departamentos.Select(d => d.Id).ToList(),
            Departamentos = departamentos.Select(d => d.Nombre).ToList(),
            Activo = municipio.Activo
        };
    }
}
