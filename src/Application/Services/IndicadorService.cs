using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class IndicadorService : IIndicadorService
{
    private readonly IIndicadorRepository _indicadorRepository;
    private readonly ILogger<IndicadorService> _logger;

    public IndicadorService(
        IIndicadorRepository indicadorRepository,
        ILogger<IndicadorService> logger)
    {
        _indicadorRepository = indicadorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<IndicadorViewModel>> GetAllIndicadoresAsync(bool incluirInactivas = false)
    {
        var indicadores = await _indicadorRepository.GetAllAsync(incluirInactivas);
        return indicadores.Select(MapToViewModel);
    }

    public async Task<IndicadorViewModel?> GetIndicadorByIdAsync(int id)
    {
        var indicador = await _indicadorRepository.GetByIdAsync(id);
        return indicador != null ? MapToViewModel(indicador) : null;
    }

    public async Task<IndicadorViewModel> CreateIndicadorAsync(IndicadorViewModel model)
    {
        if (await _indicadorRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un indicador con ese código");
        }

        var indicador = new Indicador
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            UnidadMedida = model.UnidadMedida,
            LineaBase = model.LineaBase,
            MetaFinal = model.MetaFinal,
            ResponsableId = model.ResponsableId,
            ProductoId = model.ProductoId,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _indicadorRepository.CreateAsync(indicador);
        _logger.LogInformation($"Indicador creado: {indicador.Codigo}");

        return MapToViewModel(indicador);
    }

    public async Task UpdateIndicadorAsync(int id, IndicadorViewModel model)
    {
        var indicador = await _indicadorRepository.GetByIdAsync(id);
        if (indicador == null)
        {
            throw new InvalidOperationException("Indicador no encontrado");
        }

        if (await _indicadorRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un indicador con ese código");
        }

        indicador.AlcaldiaId = model.AlcaldiaId;
        indicador.Codigo = model.Codigo;
        indicador.Nombre = model.Nombre;
        indicador.Descripcion = model.Descripcion;
        indicador.UnidadMedida = model.UnidadMedida;
        indicador.LineaBase = model.LineaBase;
        indicador.MetaFinal = model.MetaFinal;
        indicador.ResponsableId = model.ResponsableId;
        indicador.ProductoId = model.ProductoId;
        indicador.Activo = model.Activo;
        indicador.FechaActualizacion = DateTime.UtcNow;

        await _indicadorRepository.UpdateAsync(indicador);
        _logger.LogInformation($"Indicador actualizado: {indicador.Codigo}");
    }

    public async Task DeleteIndicadorAsync(int id)
    {
        await _indicadorRepository.DeleteAsync(id);
        _logger.LogInformation($"Indicador desactivado: ID {id}");
    }

    public async Task<IEnumerable<IndicadorViewModel>> SearchIndicadoresAsync(string searchTerm)
    {
        var indicadores = await _indicadorRepository.SearchAsync(searchTerm);
        return indicadores.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _indicadorRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static IndicadorViewModel MapToViewModel(Indicador indicador)
    {
        return new IndicadorViewModel
        {
            Id = indicador.Id,
            AlcaldiaId = indicador.AlcaldiaId,
            NitAlcaldia = indicador.Alcaldia?.Nit,
            MunicipioAlcaldia = indicador.Alcaldia?.Municipio?.Nombre,
            Codigo = indicador.Codigo,
            Nombre = indicador.Nombre,
            Descripcion = indicador.Descripcion,
            UnidadMedida = indicador.UnidadMedida,
            LineaBase = indicador.LineaBase,
            MetaFinal = indicador.MetaFinal,
            ResponsableId = indicador.ResponsableId,
            NombreResponsable = indicador.Responsable?.NombreCompleto,
            ProductoId = indicador.ProductoId,
            CodigoProducto = indicador.Producto?.Codigo,
            NombreProducto = indicador.Producto?.Nombre,
            Activo = indicador.Activo
        };
    }
}
