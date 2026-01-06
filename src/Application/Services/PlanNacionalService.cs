using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class PlanNacionalService : IPlanNacionalService
{
    private readonly IPlanNacionalRepository _planNacionalRepository;
    private readonly ILogger<PlanNacionalService> _logger;

    public PlanNacionalService(
        IPlanNacionalRepository planNacionalRepository,
        ILogger<PlanNacionalService> logger)
    {
        _planNacionalRepository = planNacionalRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlanNacionalViewModel>> GetAllPlanesNacionalesAsync(bool incluirInactivas = false)
    {
        var planesNacionales = await _planNacionalRepository.GetAllAsync(incluirInactivas);
        return planesNacionales.Select(MapToViewModel);
    }

    public async Task<PlanNacionalViewModel?> GetPlanNacionalByIdAsync(int id)
    {
        var planNacional = await _planNacionalRepository.GetByIdAsync(id);
        return planNacional != null ? MapToViewModel(planNacional) : null;
    }

    public async Task<PlanNacionalViewModel> CreatePlanNacionalAsync(PlanNacionalViewModel model)
    {
        if (await _planNacionalRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un plan nacional con ese código");
        }

        var planNacional = new PlanNacional
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Eje = model.Eje,
            SectorId = model.SectorId,
            FechaInicio = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null,
            FechaFin = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _planNacionalRepository.CreateAsync(planNacional);
        _logger.LogInformation($"Plan nacional creado: {planNacional.Codigo}");

        return MapToViewModel(planNacional);
    }

    public async Task UpdatePlanNacionalAsync(int id, PlanNacionalViewModel model)
    {
        var planNacional = await _planNacionalRepository.GetByIdAsync(id);
        if (planNacional == null)
        {
            throw new InvalidOperationException("Plan nacional no encontrado");
        }

        if (await _planNacionalRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un plan nacional con ese código");
        }

        planNacional.AlcaldiaId = model.AlcaldiaId;
        planNacional.Codigo = model.Codigo;
        planNacional.Nombre = model.Nombre;
        planNacional.Descripcion = model.Descripcion;
        planNacional.Eje = model.Eje;
        planNacional.SectorId = model.SectorId;
        planNacional.FechaInicio = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null;
        planNacional.FechaFin = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null;
        planNacional.Activo = model.Activo;
        planNacional.FechaActualizacion = DateTime.UtcNow;

        await _planNacionalRepository.UpdateAsync(planNacional);
        _logger.LogInformation($"Plan nacional actualizado: {planNacional.Codigo}");
    }

    public async Task DeletePlanNacionalAsync(int id)
    {
        await _planNacionalRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Plan nacional desactivado: ID {id}");
    }

    public async Task<IEnumerable<PlanNacionalViewModel>> SearchPlanesNacionalesAsync(string searchTerm)
    {
        var planesNacionales = await _planNacionalRepository.SearchAsync(searchTerm);
        return planesNacionales.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _planNacionalRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static PlanNacionalViewModel MapToViewModel(PlanNacional planNacional)
    {
        return new PlanNacionalViewModel
        {
            Id = planNacional.Id,
            AlcaldiaId = planNacional.AlcaldiaId,
            NitAlcaldia = planNacional.Alcaldia?.Nit,
            MunicipioAlcaldia = planNacional.Alcaldia?.Municipio?.Nombre,
            Codigo = planNacional.Codigo,
            Nombre = planNacional.Nombre,
            Descripcion = planNacional.Descripcion,
            Eje = planNacional.Eje,
            SectorId = planNacional.SectorId,
            CodigoSector = planNacional.Sector?.Codigo,
            NombreSector = planNacional.Sector?.Nombre,
            FechaInicio = planNacional.FechaInicio,
            FechaFin = planNacional.FechaFin,
            Activo = planNacional.Activo,
            CantidadPlanesMunicipales = planNacional.PlanesMunicipales?.Count ?? 0
        };
    }
}
