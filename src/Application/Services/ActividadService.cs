using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ActividadService : IActividadService
{
    private readonly IActividadRepository _actividadRepository;
    private readonly ILogger<ActividadService> _logger;

    public ActividadService(
        IActividadRepository actividadRepository,
        ILogger<ActividadService> logger)
    {
        _actividadRepository = actividadRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ActividadViewModel>> GetAllActividadesAsync(bool incluirInactivas = false)
    {
        var actividades = await _actividadRepository.GetAllAsync(incluirInactivas);
        return actividades.Select(MapToViewModel);
    }

    public async Task<ActividadViewModel?> GetActividadByIdAsync(int id)
    {
        var actividad = await _actividadRepository.GetByIdAsync(id);
        return actividad != null ? MapToViewModel(actividad) : null;
    }

    public async Task<ActividadViewModel> CreateActividadAsync(ActividadViewModel model)
    {
        if (await _actividadRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe una actividad con ese código");
        }

        var actividad = new Actividad
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            ProyectoId = model.ProyectoId,
            ResponsableId = model.ResponsableId,
            VigenciaId = model.VigenciaId,
            TipoActividad = model.TipoActividad,
            FechaInicioProgramada = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null,
            FechaFinProgramada = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null,
            MetaPlaneada = model.MetaPlaneada,
            PorcentajeAvance = model.PorcentajeAvance,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _actividadRepository.CreateAsync(actividad);
        _logger.LogInformation($"Actividad creada: {actividad.Codigo}");

        return MapToViewModel(actividad);
    }

    public async Task UpdateActividadAsync(int id, ActividadViewModel model)
    {
        var actividad = await _actividadRepository.GetByIdAsync(id);
        if (actividad == null)
        {
            throw new InvalidOperationException("Actividad no encontrada");
        }

        if (await _actividadRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe una actividad con ese código");
        }

        actividad.AlcaldiaId = model.AlcaldiaId;
        actividad.Codigo = model.Codigo;
        actividad.Nombre = model.Nombre;
        actividad.Descripcion = model.Descripcion;
        actividad.ProyectoId = model.ProyectoId;
        actividad.ResponsableId = model.ResponsableId;
        actividad.VigenciaId = model.VigenciaId;
        actividad.TipoActividad = model.TipoActividad;
        actividad.FechaInicioProgramada = model.FechaInicio.HasValue ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) : null;
        actividad.FechaFinProgramada = model.FechaFin.HasValue ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) : null;
        actividad.MetaPlaneada = model.MetaPlaneada;
        actividad.PorcentajeAvance = model.PorcentajeAvance;
        actividad.Activo = model.Activo;
        actividad.FechaActualizacion = DateTime.UtcNow;

        await _actividadRepository.UpdateAsync(actividad);
        _logger.LogInformation($"Actividad actualizada: {actividad.Codigo}");
    }

    public async Task DeleteActividadAsync(int id)
    {
        await _actividadRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Actividad desactivada: ID {id}");
    }

    public async Task<IEnumerable<ActividadViewModel>> SearchActividadesAsync(string searchTerm)
    {
        var actividades = await _actividadRepository.SearchAsync(searchTerm);
        return actividades.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _actividadRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static ActividadViewModel MapToViewModel(Actividad actividad)
    {
        return new ActividadViewModel
        {
            Id = actividad.Id,
            AlcaldiaId = actividad.AlcaldiaId,
            NitAlcaldia = actividad.Alcaldia?.Nit,
            MunicipioAlcaldia = actividad.Alcaldia?.Municipio?.Nombre,
            Codigo = actividad.Codigo,
            Nombre = actividad.Nombre,
            Descripcion = actividad.Descripcion,
            ProyectoId = actividad.ProyectoId,
            CodigoProyecto = actividad.Proyecto?.Codigo,
            NombreProyecto = actividad.Proyecto?.Nombre,
            ResponsableId = actividad.ResponsableId,
            NombreResponsable = actividad.Responsable?.NombreCompleto,
            VigenciaId = actividad.VigenciaId,
            NombreVigencia = actividad.Vigencia?.Nombre,
            TipoActividad = actividad.TipoActividad,
            FechaInicio = actividad.FechaInicioProgramada,
            FechaFin = actividad.FechaFinProgramada,
            MetaPlaneada = actividad.MetaPlaneada,
            PorcentajeAvance = actividad.PorcentajeAvance,
            Activo = actividad.Activo
        };
    }
}
