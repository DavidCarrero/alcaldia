using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ProyectoRepository : IProyectoRepository
{
    private readonly ApplicationDbContext _context;

    public ProyectoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Proyecto>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Proyectos
            .Include(p => p.Alcaldia)
            .Include(p => p.Responsable)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<Proyecto?> GetByIdAsync(int id)
    {
        return await _context.Proyectos
            .Include(p => p.Alcaldia)
            .Include(p => p.Responsable)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Proyecto> CreateAsync(Proyecto proyecto)
    {
        _context.Proyectos.Add(proyecto);
        await _context.SaveChangesAsync();
        return proyecto;
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        _context.Proyectos.Update(proyecto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var proyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.Id == id);
            
        if (proyecto != null)
        {
            // Soft delete
            proyecto.IsDeleted = true;
            proyecto.DeletedAt = DateTime.UtcNow;
            proyecto.DeletedBy = deletedBy;
            proyecto.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Proyecto>> SearchAsync(string searchTerm)
    {
        return await _context.Proyectos
            .Include(p => p.Alcaldia)
            .Include(p => p.Responsable)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Proyectos
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Proyectos.CountAsync(p => p.Activo);
    }
}
