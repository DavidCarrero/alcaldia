using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class VigenciaService : IVigenciaService
{
    private readonly IVigenciaRepository _vigenciaRepository;
    private readonly ILogger<VigenciaService> _logger;

    public VigenciaService(
        IVigenciaRepository vigenciaRepository,
        ILogger<VigenciaService> logger)
    {
        _vigenciaRepository = vigenciaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<VigenciaViewModel>> GetAllVigenciasAsync(bool incluirInactivas = false)
    {
        var vigencias = await _vigenciaRepository.GetAllAsync(incluirInactivas);
        return vigencias.Select(MapToViewModel);
    }

    public async Task<VigenciaViewModel?> GetVigenciaByIdAsync(int id)
    {
        var vigencia = await _vigenciaRepository.GetByIdAsync(id);
        return vigencia != null ? MapToViewModel(vigencia) : null;
    }

    public async Task<VigenciaViewModel> CreateVigenciaAsync(VigenciaViewModel model)
    {
        if (!string.IsNullOrEmpty(model.Codigo) && await _vigenciaRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe una vigencia con ese código");
        }

        var vigencia = new Vigencia
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Año = model.Año,
            FechaInicio = model.FechaInicio.HasValue 
                ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) 
                : null,
            FechaFin = model.FechaFin.HasValue 
                ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) 
                : null,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _vigenciaRepository.CreateAsync(vigencia);
        _logger.LogInformation($"Vigencia creada: {vigencia.Nombre}");

        return MapToViewModel(vigencia);
    }

    public async Task UpdateVigenciaAsync(int id, VigenciaViewModel model)
    {
        var vigencia = await _vigenciaRepository.GetByIdAsync(id);
        if (vigencia == null)
        {
            throw new InvalidOperationException("Vigencia no encontrada");
        }

        if (!string.IsNullOrEmpty(model.Codigo) && await _vigenciaRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe una vigencia con ese código");
        }

        vigencia.AlcaldiaId = model.AlcaldiaId;
        vigencia.Codigo = model.Codigo;
        vigencia.Nombre = model.Nombre;
        vigencia.Descripcion = model.Descripcion;
        vigencia.Año = model.Año;
        vigencia.FechaInicio = model.FechaInicio.HasValue 
            ? DateTime.SpecifyKind(model.FechaInicio.Value, DateTimeKind.Utc) 
            : null;
        vigencia.FechaFin = model.FechaFin.HasValue 
            ? DateTime.SpecifyKind(model.FechaFin.Value, DateTimeKind.Utc) 
            : null;
        vigencia.Activo = model.Activo;
        vigencia.FechaActualizacion = DateTime.UtcNow;

        await _vigenciaRepository.UpdateAsync(vigencia);
        _logger.LogInformation($"Vigencia actualizada: {vigencia.Nombre}");
    }

    public async Task DeleteVigenciaAsync(int id)
    {
        await _vigenciaRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Vigencia desactivada: ID {id}");
    }

    public async Task<IEnumerable<VigenciaViewModel>> SearchVigenciasAsync(string searchTerm)
    {
        var vigencias = await _vigenciaRepository.SearchAsync(searchTerm);
        return vigencias.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _vigenciaRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static VigenciaViewModel MapToViewModel(Vigencia vigencia)
    {
        return new VigenciaViewModel
        {
            Id = vigencia.Id,
            AlcaldiaId = vigencia.AlcaldiaId,
            NitAlcaldia = vigencia.Alcaldia?.Nit,
            MunicipioAlcaldia = vigencia.Alcaldia?.Municipio?.Nombre,
            Codigo = vigencia.Codigo,
            Nombre = vigencia.Nombre,
            Descripcion = vigencia.Descripcion,
            Año = vigencia.Año,
            FechaInicio = vigencia.FechaInicio,
            FechaFin = vigencia.FechaFin,
            Activo = vigencia.Activo
        };
    }
}
