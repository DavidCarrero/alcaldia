using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ProgramaService : IProgramaService
{
    private readonly IProgramaRepository _programaRepository;
    private readonly ILogger<ProgramaService> _logger;

    public ProgramaService(
        IProgramaRepository programaRepository,
        ILogger<ProgramaService> logger)
    {
        _programaRepository = programaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProgramaViewModel>> GetAllProgramasAsync(bool incluirInactivas = false)
    {
        var programas = await _programaRepository.GetAllAsync(incluirInactivas);
        return programas.Select(MapToViewModel);
    }

    public async Task<ProgramaViewModel?> GetProgramaByIdAsync(int id)
    {
        var programa = await _programaRepository.GetByIdAsync(id);
        return programa != null ? MapToViewModel(programa) : null;
    }

    public async Task<ProgramaViewModel> CreateProgramaAsync(ProgramaViewModel model)
    {
        if (await _programaRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un programa con ese código");
        }

        var programa = new Programa
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            PresupuestoCuatrienio = model.PresupuestoCuatrienio,
            PlanMunicipalId = model.PlanMunicipalId,
            SectorId = model.SectorId,
            ODSId = model.ODSId,
            MetaResultado = model.MetaResultado,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _programaRepository.CreateAsync(programa);
        _logger.LogInformation($"Programa creado: {programa.Codigo}");

        return MapToViewModel(programa);
    }

    public async Task UpdateProgramaAsync(int id, ProgramaViewModel model)
    {
        var programa = await _programaRepository.GetByIdAsync(id);
        if (programa == null)
        {
            throw new InvalidOperationException("Programa no encontrado");
        }

        if (await _programaRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un programa con ese código");
        }

        programa.AlcaldiaId = model.AlcaldiaId;
        programa.Codigo = model.Codigo;
        programa.Nombre = model.Nombre;
        programa.Descripcion = model.Descripcion;
        programa.PresupuestoCuatrienio = model.PresupuestoCuatrienio;
        programa.PlanMunicipalId = model.PlanMunicipalId;
        programa.SectorId = model.SectorId;
        programa.ODSId = model.ODSId;
        programa.MetaResultado = model.MetaResultado;
        programa.Activo = model.Activo;
        programa.FechaActualizacion = DateTime.UtcNow;

        await _programaRepository.UpdateAsync(programa);
        _logger.LogInformation($"Programa actualizado: {programa.Codigo}");
    }

    public async Task DeleteProgramaAsync(int id)
    {
        await _programaRepository.DeleteAsync(id);
        _logger.LogInformation($"Programa desactivado: ID {id}");
    }

    public async Task<IEnumerable<ProgramaViewModel>> SearchProgramasAsync(string searchTerm)
    {
        var programas = await _programaRepository.SearchAsync(searchTerm);
        return programas.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _programaRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static ProgramaViewModel MapToViewModel(Programa programa)
    {
        return new ProgramaViewModel
        {
            Id = programa.Id,
            AlcaldiaId = programa.AlcaldiaId,
            NitAlcaldia = programa.Alcaldia?.Nit,
            MunicipioAlcaldia = programa.Alcaldia?.Municipio?.Nombre,
            Codigo = programa.Codigo,
            Nombre = programa.Nombre,
            Descripcion = programa.Descripcion,
            PresupuestoCuatrienio = programa.PresupuestoCuatrienio,
            PlanMunicipalId = programa.PlanMunicipalId,
            CodigoPlanMunicipal = programa.PlanMunicipal?.Codigo,
            NombrePlanMunicipal = programa.PlanMunicipal?.Nombre,
            SectorId = programa.SectorId,
            CodigoSector = programa.Sector?.Codigo,
            NombreSector = programa.Sector?.Nombre,
            ODSId = programa.ODSId,
            CodigoODS = programa.ODS?.Codigo,
            NombreODS = programa.ODS?.Nombre,
            MetaResultado = programa.MetaResultado,
            Activo = programa.Activo,
            CantidadProductos = programa.Productos?.Count ?? 0
        };
    }
}
