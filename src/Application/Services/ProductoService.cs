using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(
        IProductoRepository productoRepository,
        ILogger<ProductoService> logger)
    {
        _productoRepository = productoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductoViewModel>> GetAllProductosAsync(bool incluirInactivas = false)
    {
        var productos = await _productoRepository.GetAllAsync(incluirInactivas);
        return productos.Select(MapToViewModel);
    }

    public async Task<ProductoViewModel?> GetProductoByIdAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        return producto != null ? MapToViewModel(producto) : null;
    }

    public async Task<ProductoViewModel> CreateProductoAsync(ProductoViewModel model)
    {
        if (await _productoRepository.CodigoExistsAsync(model.Codigo))
        {
            throw new InvalidOperationException("Ya existe un producto con ese código");
        }

        var producto = new Producto
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            ProgramaId = model.ProgramaId,
            PresupuestoAsignado = model.PresupuestoAsignado,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _productoRepository.CreateAsync(producto);
        _logger.LogInformation($"Producto creado: {producto.Codigo}");

        return MapToViewModel(producto);
    }

    public async Task UpdateProductoAsync(int id, ProductoViewModel model)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null)
        {
            throw new InvalidOperationException("Producto no encontrado");
        }

        if (await _productoRepository.CodigoExistsAsync(model.Codigo, id))
        {
            throw new InvalidOperationException("Ya existe un producto con ese código");
        }

        producto.AlcaldiaId = model.AlcaldiaId;
        producto.Codigo = model.Codigo;
        producto.Nombre = model.Nombre;
        producto.Descripcion = model.Descripcion;
        producto.ProgramaId = model.ProgramaId;
        producto.PresupuestoAsignado = model.PresupuestoAsignado;
        producto.Activo = model.Activo;
        producto.FechaActualizacion = DateTime.UtcNow;

        await _productoRepository.UpdateAsync(producto);
        _logger.LogInformation($"Producto actualizado: {producto.Codigo}");
    }

    public async Task DeleteProductoAsync(int id)
    {
        await _productoRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Producto desactivado: ID {id}");
    }

    public async Task<IEnumerable<ProductoViewModel>> SearchProductosAsync(string searchTerm)
    {
        var productos = await _productoRepository.SearchAsync(searchTerm);
        return productos.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _productoRepository.CodigoExistsAsync(codigo, excludeId);
    }

    private static ProductoViewModel MapToViewModel(Producto producto)
    {
        return new ProductoViewModel
        {
            Id = producto.Id,
            AlcaldiaId = producto.AlcaldiaId,
            NitAlcaldia = producto.Alcaldia?.Nit,
            MunicipioAlcaldia = producto.Alcaldia?.Municipio?.Nombre,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            ProgramaId = producto.ProgramaId,
            CodigoPrograma = producto.Programa?.Codigo,
            NombrePrograma = producto.Programa?.Nombre,
            PresupuestoAsignado = producto.PresupuestoAsignado,
            Activo = producto.Activo,
            CantidadIndicadores = producto.Indicadores?.Count ?? 0
        };
    }
}
