using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _proyectoRepository;
    private readonly ILogger<ProyectoService> _logger;

    public ProyectoService(
        IProyectoRepository proyectoRepository,
        ILogger<ProyectoService> logger)
    {
        _proyectoRepository = proyectoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProyectoViewModel>> GetAllProyectosAsync(bool incluirInactivas = false)
    {
        var proyectos = await _proyectoRepository.GetAllAsync(incluirInactivas);
        return proyectos.Select(MapToViewModel);
    }

    public async Task<ProyectoViewModel?> GetProyectoByIdAsync(int id)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        return proyecto != null ? MapToViewModel(proyecto) : null;
    }

    public async Task<ProyectoViewModel> CreateProyectoAsync(ProyectoViewModel model)
    {
        if (await _proyectoRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un proyecto con ese código");
        }

        var proyecto = new Proyecto
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            CodigoBPIN = model.CodigoBPIN,
            PresupuestoAsignado = model.PresupuestoAsignado,
            PresupuestoEjecutado = model.PresupuestoEjecutado,
            ValorProyecto = model.ValorProyecto,
            ResponsableId = model.ResponsableId,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            EstadoProyecto = model.EstadoProyecto,
            PorcentajeAvance = model.PorcentajeAvance,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _proyectoRepository.CreateAsync(proyecto);
        _logger.LogInformation($"Proyecto creado: {proyecto.Codigo}");

        return MapToViewModel(proyecto);
    }

    public async Task UpdateProyectoAsync(int id, ProyectoViewModel model)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id);
        if (proyecto == null)
        {
            throw new InvalidOperationException("Proyecto no encontrado");
        }

        if (await _proyectoRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un proyecto con ese código");
        }

        proyecto.AlcaldiaId = model.AlcaldiaId;
        proyecto.Codigo = model.Codigo;
        proyecto.Nombre = model.Nombre;
        proyecto.Descripcion = model.Descripcion;
        proyecto.CodigoBPIN = model.CodigoBPIN;
        proyecto.PresupuestoAsignado = model.PresupuestoAsignado;
        proyecto.PresupuestoEjecutado = model.PresupuestoEjecutado;
        proyecto.ValorProyecto = model.ValorProyecto;
        proyecto.ResponsableId = model.ResponsableId;
        proyecto.FechaInicio = model.FechaInicio;
        proyecto.FechaFin = model.FechaFin;
        proyecto.EstadoProyecto = model.EstadoProyecto;
        proyecto.PorcentajeAvance = model.PorcentajeAvance;
        proyecto.Activo = model.Activo;
        proyecto.FechaActualizacion = DateTime.UtcNow;

        await _proyectoRepository.UpdateAsync(proyecto);
        _logger.LogInformation($"Proyecto actualizado: {proyecto.Codigo}");
    }

    public async Task DeleteProyectoAsync(int id)
    {
        await _proyectoRepository.DeleteAsync(id);
        _logger.LogInformation($"Proyecto desactivado: ID {id}");
    }

    public async Task<IEnumerable<ProyectoViewModel>> SearchProyectosAsync(string searchTerm)
    {
        var proyectos = await _proyectoRepository.SearchAsync(searchTerm);
        return proyectos.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _proyectoRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static ProyectoViewModel MapToViewModel(Proyecto proyecto)
    {
        return new ProyectoViewModel
        {
            Id = proyecto.Id,
            AlcaldiaId = proyecto.AlcaldiaId,
            NitAlcaldia = proyecto.Alcaldia?.Nit,
            MunicipioAlcaldia = proyecto.Alcaldia?.Municipio?.Nombre,
            Codigo = proyecto.Codigo,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            CodigoBPIN = proyecto.CodigoBPIN,
            PresupuestoAsignado = proyecto.PresupuestoAsignado,
            PresupuestoEjecutado = proyecto.PresupuestoEjecutado,
            ValorProyecto = proyecto.ValorProyecto,
            ResponsableId = proyecto.ResponsableId,
            NombreResponsable = proyecto.Responsable?.NombreCompleto,
            FechaInicio = proyecto.FechaInicio,
            FechaFin = proyecto.FechaFin,
            EstadoProyecto = proyecto.EstadoProyecto,
            PorcentajeAvance = proyecto.PorcentajeAvance,
            Activo = proyecto.Activo,
            CantidadActividades = proyecto.Actividades?.Count ?? 0
        };
    }
}
