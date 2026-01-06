using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class LineaEstrategicaRepository : ILineaEstrategicaRepository
{
    private readonly ApplicationDbContext _context;

    public LineaEstrategicaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LineaEstrategica>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.LineasEstrategicas
            .Include(l => l.Alcaldia)
            .Include(l => l.Sectores)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(l => l.Activo);
        }

        return await query.OrderBy(l => l.Codigo).ToListAsync();
    }

    public async Task<LineaEstrategica?> GetByIdAsync(int id)
    {
        return await _context.LineasEstrategicas
            .Include(l => l.Alcaldia)
            .Include(l => l.Sectores)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<LineaEstrategica> CreateAsync(LineaEstrategica lineaEstrategica)
    {
        _context.LineasEstrategicas.Add(lineaEstrategica);
        await _context.SaveChangesAsync();
        return lineaEstrategica;
    }

    public async Task UpdateAsync(LineaEstrategica lineaEstrategica)
    {
        _context.LineasEstrategicas.Update(lineaEstrategica);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var lineaEstrategica = await _context.LineasEstrategicas.FirstOrDefaultAsync(l => l.Id == id);
            
        if (lineaEstrategica != null)
        {
            // Soft delete
            lineaEstrategica.IsDeleted = true;
            lineaEstrategica.DeletedAt = DateTime.UtcNow;
            lineaEstrategica.DeletedBy = deletedBy;
            lineaEstrategica.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<LineaEstrategica>> SearchAsync(string searchTerm)
    {
        return await _context.LineasEstrategicas
            .Include(l => l.Alcaldia)
            .Include(l => l.Sectores)
            .Where(l => l.Codigo.Contains(searchTerm) || l.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.LineasEstrategicas
            .AnyAsync(l => l.Codigo == codigo && (!excludeId.HasValue || l.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.LineasEstrategicas.CountAsync(l => l.Activo);
    }
}
