using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class LineaEstrategicaService : ILineaEstrategicaService
{
    private readonly ILineaEstrategicaRepository _lineaEstrategicaRepository;
    private readonly ILogger<LineaEstrategicaService> _logger;

    public LineaEstrategicaService(
        ILineaEstrategicaRepository lineaEstrategicaRepository,
        ILogger<LineaEstrategicaService> logger)
    {
        _lineaEstrategicaRepository = lineaEstrategicaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<LineaEstrategicaViewModel>> GetAllLineasEstrategicasAsync(bool incluirInactivas = false)
    {
        var lineasEstrategicas = await _lineaEstrategicaRepository.GetAllAsync(incluirInactivas);
        return lineasEstrategicas.Select(MapToViewModel);
    }

    public async Task<LineaEstrategicaViewModel?> GetLineaEstrategicaByIdAsync(int id)
    {
        var lineaEstrategica = await _lineaEstrategicaRepository.GetByIdAsync(id);
        return lineaEstrategica != null ? MapToViewModel(lineaEstrategica) : null;
    }

    public async Task<LineaEstrategicaViewModel> CreateLineaEstrategicaAsync(LineaEstrategicaViewModel model)
    {
        if (await _lineaEstrategicaRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe una línea estratégica con ese código");
        }

        var lineaEstrategica = new LineaEstrategica
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Sigla = model.Sigla,
            PlanDtpId = model.PlanDtpId,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _lineaEstrategicaRepository.CreateAsync(lineaEstrategica);
        _logger.LogInformation($"Línea estratégica creada: {lineaEstrategica.Codigo}");

        return MapToViewModel(lineaEstrategica);
    }

    public async Task UpdateLineaEstrategicaAsync(int id, LineaEstrategicaViewModel model)
    {
        var lineaEstrategica = await _lineaEstrategicaRepository.GetByIdAsync(id);
        if (lineaEstrategica == null)
        {
            throw new InvalidOperationException("Línea estratégica no encontrada");
        }

        if (await _lineaEstrategicaRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe una línea estratégica con ese código");
        }

        lineaEstrategica.AlcaldiaId = model.AlcaldiaId;
        lineaEstrategica.Codigo = model.Codigo;
        lineaEstrategica.Nombre = model.Nombre;
        lineaEstrategica.Descripcion = model.Descripcion;
        lineaEstrategica.Sigla = model.Sigla;
        lineaEstrategica.PlanDtpId = model.PlanDtpId;
        lineaEstrategica.Activo = model.Activo;
        lineaEstrategica.FechaActualizacion = DateTime.UtcNow;

        await _lineaEstrategicaRepository.UpdateAsync(lineaEstrategica);
        _logger.LogInformation($"Línea estratégica actualizada: {lineaEstrategica.Codigo}");
    }

    public async Task DeleteLineaEstrategicaAsync(int id)
    {
        await _lineaEstrategicaRepository.DeleteAsync(id);
        _logger.LogInformation($"Línea estratégica desactivada: ID {id}");
    }

    public async Task<IEnumerable<LineaEstrategicaViewModel>> SearchLineasEstrategicasAsync(string searchTerm)
    {
        var lineasEstrategicas = await _lineaEstrategicaRepository.SearchAsync(searchTerm);
        return lineasEstrategicas.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _lineaEstrategicaRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static LineaEstrategicaViewModel MapToViewModel(LineaEstrategica lineaEstrategica)
    {
        return new LineaEstrategicaViewModel
        {
            Id = lineaEstrategica.Id,
            AlcaldiaId = lineaEstrategica.AlcaldiaId,
            NitAlcaldia = lineaEstrategica.Alcaldia?.Nit,
            MunicipioAlcaldia = lineaEstrategica.Alcaldia?.Municipio?.Nombre,
            Codigo = lineaEstrategica.Codigo,
            Nombre = lineaEstrategica.Nombre,
            Descripcion = lineaEstrategica.Descripcion,
            Sigla = lineaEstrategica.Sigla,
            PlanDtpId = lineaEstrategica.PlanDtpId,
            Activo = lineaEstrategica.Activo,
            CantidadSectores = lineaEstrategica.Sectores?.Count ?? 0
        };
    }
}
