using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class EvidenciaService : IEvidenciaService
{
    private readonly IEvidenciaRepository _evidenciaRepository;
    private readonly ILogger<EvidenciaService> _logger;

    public EvidenciaService(
        IEvidenciaRepository evidenciaRepository,
        ILogger<EvidenciaService> logger)
    {
        _evidenciaRepository = evidenciaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EvidenciaViewModel>> GetAllEvidenciasAsync(bool incluirInactivas = false)
    {
        var evidencias = await _evidenciaRepository.GetAllAsync(incluirInactivas);
        return evidencias.Select(MapToViewModel);
    }

    public async Task<EvidenciaViewModel?> GetEvidenciaByIdAsync(int id)
    {
        var evidencia = await _evidenciaRepository.GetByIdAsync(id);
        return evidencia != null ? MapToViewModel(evidencia) : null;
    }

    public async Task<EvidenciaViewModel> CreateEvidenciaAsync(EvidenciaViewModel model)
    {
        if (await _evidenciaRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe una evidencia con ese código");
        }

        var evidencia = new Evidencia
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            ActividadId = model.ActividadId,
            TipoEvidencia = model.TipoEvidencia,
            NombreArchivo = model.NombreArchivo,
            TipoMime = model.TipoMime,
            TamanoBytes = model.TamanoBytes,
            RutaAlmacenamiento = model.RutaArchivo,
            FechaEvidencia = model.FechaCaptura,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _evidenciaRepository.CreateAsync(evidencia);
        _logger.LogInformation($"Evidencia creada: {evidencia.Codigo}");

        return MapToViewModel(evidencia);
    }

    public async Task UpdateEvidenciaAsync(int id, EvidenciaViewModel model)
    {
        var evidencia = await _evidenciaRepository.GetByIdAsync(id);
        if (evidencia == null)
        {
            throw new InvalidOperationException("Evidencia no encontrada");
        }

        if (await _evidenciaRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe una evidencia con ese código");
        }

        evidencia.AlcaldiaId = model.AlcaldiaId;
        evidencia.Codigo = model.Codigo;
        evidencia.Nombre = model.Nombre;
        evidencia.Descripcion = model.Descripcion;
        evidencia.ActividadId = model.ActividadId;
        evidencia.TipoEvidencia = model.TipoEvidencia;
        evidencia.NombreArchivo = model.NombreArchivo;
        evidencia.TipoMime = model.TipoMime;
        evidencia.TamanoBytes = model.TamanoBytes;
        evidencia.RutaAlmacenamiento = model.RutaArchivo;
        evidencia.FechaEvidencia = model.FechaCaptura;
        evidencia.Activo = model.Activo;
        evidencia.FechaActualizacion = DateTime.UtcNow;

        await _evidenciaRepository.UpdateAsync(evidencia);
        _logger.LogInformation($"Evidencia actualizada: {evidencia.Codigo}");
    }

    public async Task DeleteEvidenciaAsync(int id)
    {
        await _evidenciaRepository.DeleteAsync(id);
        _logger.LogInformation($"Evidencia desactivada: ID {id}");
    }

    public async Task<IEnumerable<EvidenciaViewModel>> SearchEvidenciasAsync(string searchTerm)
    {
        var evidencias = await _evidenciaRepository.SearchAsync(searchTerm);
        return evidencias.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _evidenciaRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static EvidenciaViewModel MapToViewModel(Evidencia evidencia)
    {
        return new EvidenciaViewModel
        {
            Id = evidencia.Id,
            AlcaldiaId = evidencia.AlcaldiaId,
            NitAlcaldia = evidencia.Alcaldia?.Nit,
            MunicipioAlcaldia = evidencia.Alcaldia?.Municipio?.Nombre,
            Codigo = evidencia.Codigo,
            Nombre = evidencia.Nombre,
            Descripcion = evidencia.Descripcion,
            ActividadId = evidencia.ActividadId,
            CodigoActividad = evidencia.Actividad?.Codigo,
            NombreActividad = evidencia.Actividad?.Nombre,
            TipoEvidencia = evidencia.TipoEvidencia,
            NombreArchivo = evidencia.NombreArchivo,
            TipoMime = evidencia.TipoMime,
            TamanoBytes = evidencia.TamanoBytes,
            RutaArchivo = evidencia.RutaAlmacenamiento,
            FechaCaptura = evidencia.FechaEvidencia,
            Activo = evidencia.Activo
        };
    }
}
