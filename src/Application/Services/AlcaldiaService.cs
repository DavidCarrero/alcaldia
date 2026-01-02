using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class AlcaldiaService : IAlcaldiaService
{
    private readonly IAlcaldiaRepository _alcaldiaRepository;
    private readonly IMunicipioRepository _municipioRepository;
    private readonly ILogger<AlcaldiaService> _logger;

    public AlcaldiaService(
        IAlcaldiaRepository alcaldiaRepository,
        IMunicipioRepository municipioRepository,
        ILogger<AlcaldiaService> logger)
    {
        _alcaldiaRepository = alcaldiaRepository;
        _municipioRepository = municipioRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlcaldiaViewModel>> GetAllAlcaldiasAsync(bool incluirInactivas = false)
    {
        var alcaldias = await _alcaldiaRepository.GetAllAsync(incluirInactivas);
        return alcaldias.Select(MapToViewModel);
    }

    public async Task<AlcaldiaViewModel?> GetAlcaldiaByIdAsync(int id)
    {
        var alcaldia = await _alcaldiaRepository.GetByIdAsync(id);
        return alcaldia != null ? MapToViewModel(alcaldia) : null;
    }

    public async Task<AlcaldiaViewModel> CreateAlcaldiaAsync(AlcaldiaViewModel model)
    {
        if (await _alcaldiaRepository.NitExistsAsync(model.Nit))
        {
            throw new InvalidOperationException("Ya existe una alcaldía con ese NIT");
        }

        var alcaldia = new Alcaldia
        {
            Nit = model.Nit,
            MunicipioId = model.MunicipioId,
            Direccion = model.Direccion,
            Telefono = model.Telefono,
            CorreoInstitucional = model.CorreoInstitucional,
            Logo = model.Logo,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _alcaldiaRepository.CreateAsync(alcaldia);
        _logger.LogInformation($"Alcaldía creada: {alcaldia.Nit}");

        return MapToViewModel(alcaldia);
    }

    public async Task UpdateAlcaldiaAsync(int id, AlcaldiaViewModel model)
    {
        var alcaldia = await _alcaldiaRepository.GetByIdAsync(id);
        if (alcaldia == null)
        {
            throw new InvalidOperationException("Alcaldía no encontrada");
        }

        if (await _alcaldiaRepository.NitExistsAsync(model.Nit, id))
        {
            throw new InvalidOperationException("Ya existe una alcaldía con ese NIT");
        }

        alcaldia.Nit = model.Nit;
        alcaldia.MunicipioId = model.MunicipioId;
        alcaldia.Direccion = model.Direccion;
        alcaldia.Telefono = model.Telefono;
        alcaldia.CorreoInstitucional = model.CorreoInstitucional;
        alcaldia.Logo = model.Logo;
        alcaldia.Activo = model.Activo;
        alcaldia.FechaActualizacion = DateTime.UtcNow;

        await _alcaldiaRepository.UpdateAsync(alcaldia);
        _logger.LogInformation($"Alcaldía actualizada: {alcaldia.Nit}");
    }

    public async Task DeleteAlcaldiaAsync(int id)
    {
        await _alcaldiaRepository.DeleteAsync(id);
        _logger.LogInformation($"Alcaldía desactivada: ID {id}");
    }

    public async Task<IEnumerable<AlcaldiaViewModel>> SearchAlcaldiasAsync(string searchTerm)
    {
        var alcaldias = await _alcaldiaRepository.SearchAsync(searchTerm);
        return alcaldias.Select(MapToViewModel);
    }

    public async Task<bool> NitExistsAsync(string nit, int? excludeId = null)
    {
        return await _alcaldiaRepository.NitExistsAsync(nit, excludeId);
    }

    private static AlcaldiaViewModel MapToViewModel(Alcaldia alcaldia)
    {
        return new AlcaldiaViewModel
        {
            Id = alcaldia.Id,
            Nit = alcaldia.Nit,
            MunicipioId = alcaldia.MunicipioId ?? 0,
            CodigoMunicipio = alcaldia.Municipio?.Codigo,
            NombreMunicipio = alcaldia.Municipio?.Nombre,
            NombreDepartamento = alcaldia.Municipio?.Departamentos?.FirstOrDefault()?.Nombre,
            Direccion = alcaldia.Direccion,
            Telefono = alcaldia.Telefono,
            CorreoInstitucional = alcaldia.CorreoInstitucional,
            Logo = alcaldia.Logo,
            Activo = alcaldia.Activo
        };
    }
}
