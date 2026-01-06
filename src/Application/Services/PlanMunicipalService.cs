using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class PlanMunicipalService : IPlanMunicipalService
{
    private readonly IPlanMunicipalRepository _planMunicipalRepository;
    private readonly ILogger<PlanMunicipalService> _logger;

    public PlanMunicipalService(
        IPlanMunicipalRepository planMunicipalRepository,
        ILogger<PlanMunicipalService> logger)
    {
        _planMunicipalRepository = planMunicipalRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlanMunicipalViewModel>> GetAllPlanesMunicipalesAsync(bool incluirInactivas = false)
    {
        var planesMunicipales = await _planMunicipalRepository.GetAllAsync(incluirInactivas);
        return planesMunicipales.Select(MapToViewModel);
    }

    public async Task<PlanMunicipalViewModel?> GetPlanMunicipalByIdAsync(int id)
    {
        var planMunicipal = await _planMunicipalRepository.GetByIdAsync(id);
        return planMunicipal != null ? MapToViewModel(planMunicipal) : null;
    }

    public async Task<PlanMunicipalViewModel> CreatePlanMunicipalAsync(PlanMunicipalViewModel model)
    {
        if (await _planMunicipalRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un plan municipal con ese código");
        }

        var planMunicipal = new PlanMunicipal
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            MunicipioId = model.MunicipioId,
            AlcaldeId = model.AlcaldeId,
            PlanNacionalId = model.PlanNacionalId,
            PlanDptlId = model.PlanDptlId,
            FechaInicio = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null,
            FechaFin = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _planMunicipalRepository.CreateAsync(planMunicipal);
        _logger.LogInformation($"Plan municipal creado: {planMunicipal.Codigo}");

        return MapToViewModel(planMunicipal);
    }

    public async Task UpdatePlanMunicipalAsync(int id, PlanMunicipalViewModel model)
    {
        var planMunicipal = await _planMunicipalRepository.GetByIdAsync(id);
        if (planMunicipal == null)
        {
            throw new InvalidOperationException("Plan municipal no encontrado");
        }

        if (await _planMunicipalRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un plan municipal con ese código");
        }

        planMunicipal.AlcaldiaId = model.AlcaldiaId;
        planMunicipal.Codigo = model.Codigo;
        planMunicipal.Nombre = model.Nombre;
        planMunicipal.Descripcion = model.Descripcion;
        planMunicipal.MunicipioId = model.MunicipioId;
        planMunicipal.AlcaldeId = model.AlcaldeId;
        planMunicipal.PlanNacionalId = model.PlanNacionalId;
        planMunicipal.PlanDptlId = model.PlanDptlId;
        planMunicipal.FechaInicio = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null;
        planMunicipal.FechaFin = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null;
        planMunicipal.Activo = model.Activo;
        planMunicipal.FechaActualizacion = DateTime.UtcNow;

        await _planMunicipalRepository.UpdateAsync(planMunicipal);
        _logger.LogInformation($"Plan municipal actualizado: {planMunicipal.Codigo}");
    }

    public async Task DeletePlanMunicipalAsync(int id)
    {
        await _planMunicipalRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Plan municipal desactivado: ID {id}");
    }

    public async Task<IEnumerable<PlanMunicipalViewModel>> SearchPlanesMunicipalesAsync(string searchTerm)
    {
        var planesMunicipales = await _planMunicipalRepository.SearchAsync(searchTerm);
        return planesMunicipales.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _planMunicipalRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static PlanMunicipalViewModel MapToViewModel(PlanMunicipal planMunicipal)
    {
        return new PlanMunicipalViewModel
        {
            Id = planMunicipal.Id,
            AlcaldiaId = planMunicipal.AlcaldiaId,
            NitAlcaldia = planMunicipal.Alcaldia?.Nit,
            MunicipioAlcaldia = planMunicipal.Alcaldia?.Municipio?.Nombre,
            Codigo = planMunicipal.Codigo,
            Nombre = planMunicipal.Nombre,
            Descripcion = planMunicipal.Descripcion,
            MunicipioId = planMunicipal.MunicipioId,
            CodigoMunicipio = planMunicipal.Municipio?.Codigo,
            NombreMunicipio = planMunicipal.Municipio?.Nombre,
            AlcaldeId = planMunicipal.AlcaldeId,
            NombreAlcalde = planMunicipal.Alcalde?.NombreCompleto,
            PlanNacionalId = planMunicipal.PlanNacionalId,
            CodigoPlanNacional = planMunicipal.PlanNacional?.Codigo,
            NombrePlanNacional = planMunicipal.PlanNacional?.Nombre,
            PlanDptlId = planMunicipal.PlanDptlId,
            CodigoPlanDepartamental = planMunicipal.PlanDepartamental?.Codigo,
            NombrePlanDepartamental = planMunicipal.PlanDepartamental?.Nombre,
            FechaInicio = planMunicipal.FechaInicio,
            FechaFin = planMunicipal.FechaFin,
            Activo = planMunicipal.Activo,
            CantidadProgramas = planMunicipal.Programas?.Count ?? 0
        };
    }
}
