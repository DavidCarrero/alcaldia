using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class SectorService : ISectorService
{
    private readonly ISectorRepository _sectorRepository;
    private readonly ILogger<SectorService> _logger;

    public SectorService(
        ISectorRepository sectorRepository,
        ILogger<SectorService> logger)
    {
        _sectorRepository = sectorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SectorViewModel>> GetAllSectoresAsync(bool incluirInactivas = false)
    {
        var sectores = await _sectorRepository.GetAllAsync(incluirInactivas);
        return sectores.Select(MapToViewModel);
    }

    public async Task<SectorViewModel?> GetSectorByIdAsync(int id)
    {
        var sector = await _sectorRepository.GetByIdAsync(id);
        return sector != null ? MapToViewModel(sector) : null;
    }

    public async Task<SectorViewModel> CreateSectorAsync(SectorViewModel model)
    {
        if (await _sectorRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un sector con ese código");
        }

        var sector = new Sector
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            LineaEstrategicaId = model.LineaEstrategicaId,
            Aplicacion = model.Aplicacion,
            PresupuestoAsignado = model.PresupuestoAsignado,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _sectorRepository.CreateAsync(sector);
        _logger.LogInformation($"Sector creado: {sector.Codigo}");

        return MapToViewModel(sector);
    }

    public async Task UpdateSectorAsync(int id, SectorViewModel model)
    {
        var sector = await _sectorRepository.GetByIdAsync(id);
        if (sector == null)
        {
            throw new InvalidOperationException("Sector no encontrado");
        }

        if (await _sectorRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un sector con ese código");
        }

        sector.AlcaldiaId = model.AlcaldiaId;
        sector.Codigo = model.Codigo;
        sector.Nombre = model.Nombre;
        sector.Descripcion = model.Descripcion;
        sector.LineaEstrategicaId = model.LineaEstrategicaId;
        sector.Aplicacion = model.Aplicacion;
        sector.PresupuestoAsignado = model.PresupuestoAsignado;
        sector.Activo = model.Activo;
        sector.FechaActualizacion = DateTime.UtcNow;

        await _sectorRepository.UpdateAsync(sector);
        _logger.LogInformation($"Sector actualizado: {sector.Codigo}");
    }

    public async Task DeleteSectorAsync(int id)
    {
        await _sectorRepository.DeleteAsync(id);
        _logger.LogInformation($"Sector desactivado: ID {id}");
    }

    public async Task<IEnumerable<SectorViewModel>> SearchSectoresAsync(string searchTerm)
    {
        var sectores = await _sectorRepository.SearchAsync(searchTerm);
        return sectores.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _sectorRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static SectorViewModel MapToViewModel(Sector sector)
    {
        return new SectorViewModel
        {
            Id = sector.Id,
            AlcaldiaId = sector.AlcaldiaId,
            NitAlcaldia = sector.Alcaldia?.Nit,
            MunicipioAlcaldia = sector.Alcaldia?.Municipio?.Nombre,
            Codigo = sector.Codigo,
            Nombre = sector.Nombre,
            Descripcion = sector.Descripcion,
            LineaEstrategicaId = sector.LineaEstrategicaId,
            CodigoLineaEstrategica = sector.LineaEstrategica?.Codigo,
            NombreLineaEstrategica = sector.LineaEstrategica?.Nombre,
            Aplicacion = sector.Aplicacion,
            PresupuestoAsignado = sector.PresupuestoAsignado,
            Activo = sector.Activo,
            CantidadProgramas = sector.Programas?.Count ?? 0
        };
    }
}
