using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class PlanDepartamentalService : IPlanDepartamentalService
{
    private readonly IPlanDepartamentalRepository _planDepartamentalRepository;
    private readonly ILogger<PlanDepartamentalService> _logger;

    public PlanDepartamentalService(
        IPlanDepartamentalRepository planDepartamentalRepository,
        ILogger<PlanDepartamentalService> logger)
    {
        _planDepartamentalRepository = planDepartamentalRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlanDepartamentalViewModel>> GetAllPlanesDepartamentalesAsync(bool incluirInactivas = false)
    {
        var planesDepartamentales = await _planDepartamentalRepository.GetAllAsync(incluirInactivas);
        return planesDepartamentales.Select(MapToViewModel);
    }

    public async Task<PlanDepartamentalViewModel?> GetPlanDepartamentalByIdAsync(int id)
    {
        var planDepartamental = await _planDepartamentalRepository.GetByIdAsync(id);
        return planDepartamental != null ? MapToViewModel(planDepartamental) : null;
    }

    public async Task<PlanDepartamentalViewModel> CreatePlanDepartamentalAsync(PlanDepartamentalViewModel model)
    {
        if (await _planDepartamentalRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un plan departamental con ese código");
        }

        var planDepartamental = new PlanDepartamental
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Departamento = model.Departamento,
            Eje = model.Eje,
            SectorId = model.SectorId,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _planDepartamentalRepository.CreateAsync(planDepartamental);
        _logger.LogInformation($"Plan departamental creado: {planDepartamental.Codigo}");

        return MapToViewModel(planDepartamental);
    }

    public async Task UpdatePlanDepartamentalAsync(int id, PlanDepartamentalViewModel model)
    {
        var planDepartamental = await _planDepartamentalRepository.GetByIdAsync(id);
        if (planDepartamental == null)
        {
            throw new InvalidOperationException("Plan departamental no encontrado");
        }

        if (await _planDepartamentalRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un plan departamental con ese código");
        }

        planDepartamental.AlcaldiaId = model.AlcaldiaId;
        planDepartamental.Codigo = model.Codigo;
        planDepartamental.Nombre = model.Nombre;
        planDepartamental.Descripcion = model.Descripcion;
        planDepartamental.Departamento = model.Departamento;
        planDepartamental.Eje = model.Eje;
        planDepartamental.SectorId = model.SectorId;
        planDepartamental.FechaInicio = model.FechaInicio;
        planDepartamental.FechaFin = model.FechaFin;
        planDepartamental.Activo = model.Activo;
        planDepartamental.FechaActualizacion = DateTime.UtcNow;

        await _planDepartamentalRepository.UpdateAsync(planDepartamental);
        _logger.LogInformation($"Plan departamental actualizado: {planDepartamental.Codigo}");
    }

    public async Task DeletePlanDepartamentalAsync(int id)
    {
        await _planDepartamentalRepository.DeleteAsync(id);
        _logger.LogInformation($"Plan departamental desactivado: ID {id}");
    }

    public async Task<IEnumerable<PlanDepartamentalViewModel>> SearchPlanesDepartamentalesAsync(string searchTerm)
    {
        var planesDepartamentales = await _planDepartamentalRepository.SearchAsync(searchTerm);
        return planesDepartamentales.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _planDepartamentalRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static PlanDepartamentalViewModel MapToViewModel(PlanDepartamental planDepartamental)
    {
        return new PlanDepartamentalViewModel
        {
            Id = planDepartamental.Id,
            AlcaldiaId = planDepartamental.AlcaldiaId,
            NitAlcaldia = planDepartamental.Alcaldia?.Nit,
            MunicipioAlcaldia = planDepartamental.Alcaldia?.Municipio?.Nombre,
            Codigo = planDepartamental.Codigo,
            Nombre = planDepartamental.Nombre,
            Descripcion = planDepartamental.Descripcion,
            Departamento = planDepartamental.Departamento,
            Eje = planDepartamental.Eje,
            SectorId = planDepartamental.SectorId,
            CodigoSector = planDepartamental.Sector?.Codigo,
            NombreSector = planDepartamental.Sector?.Nombre,
            FechaInicio = planDepartamental.FechaInicio,
            FechaFin = planDepartamental.FechaFin,
            Activo = planDepartamental.Activo,
            CantidadPlanesMunicipales = planDepartamental.PlanesMunicipales?.Count ?? 0
        };
    }
}
