using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Producto>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Productos
            .Include(p => p.Alcaldia)
            .Include(p => p.Programa)
            .Include(p => p.Indicadores)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Alcaldia)
            .Include(p => p.Programa)
            .Include(p => p.Indicadores)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto> CreateAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task UpdateAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            
        if (producto != null)
        {
            // Soft delete
            producto.IsDeleted = true;
            producto.DeletedAt = DateTime.UtcNow;
            producto.DeletedBy = deletedBy;
            producto.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Producto>> SearchAsync(string searchTerm)
    {
        return await _context.Productos
            .Include(p => p.Alcaldia)
            .Include(p => p.Programa)
            .Include(p => p.Indicadores)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Productos
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Productos.CountAsync(p => p.Activo);
    }
}
