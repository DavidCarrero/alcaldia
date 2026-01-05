using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Application.Services;

public interface IProductoService
{
    Task<IEnumerable<ProductoViewModel>> GetAllProductosAsync(bool incluirInactivas = false);
    Task<ProductoViewModel?> GetProductoByIdAsync(int id);
    Task<ProductoViewModel> CreateProductoAsync(ProductoViewModel model);
    Task UpdateProductoAsync(int id, ProductoViewModel model);
    Task DeleteProductoAsync(int id);
    Task<IEnumerable<ProductoViewModel>> SearchProductosAsync(string searchTerm);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
