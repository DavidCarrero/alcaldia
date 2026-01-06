using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ActividadRepository : IActividadRepository
{
    private readonly ApplicationDbContext _context;

    public ActividadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Actividad>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Actividades
            .Include(a => a.Alcaldia)
            .Include(a => a.Proyecto)
            .Include(a => a.Responsable)
            .Include(a => a.Vigencia)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(a => a.Activo);
        }

        return await query.OrderBy(a => a.Codigo).ToListAsync();
    }

    public async Task<Actividad?> GetByIdAsync(int id)
    {
        return await _context.Actividades
            .Include(a => a.Alcaldia)
            .Include(a => a.Proyecto)
            .Include(a => a.Responsable)
            .Include(a => a.Vigencia)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Actividad> CreateAsync(Actividad actividad)
    {
        _context.Actividades.Add(actividad);
        await _context.SaveChangesAsync();
        return actividad;
    }

    public async Task UpdateAsync(Actividad actividad)
    {
        _context.Actividades.Update(actividad);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var actividad = await _context.Actividades.FirstOrDefaultAsync(a => a.Id == id);
            
        if (actividad != null)
        {
            // Soft delete
            actividad.IsDeleted = true;
            actividad.DeletedAt = DateTime.UtcNow;
            actividad.DeletedBy = deletedBy;
            actividad.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Actividad>> SearchAsync(string searchTerm)
    {
        return await _context.Actividades
            .Include(a => a.Alcaldia)
            .Include(a => a.Proyecto)
            .Include(a => a.Responsable)
            .Include(a => a.Vigencia)
            .Where(a => a.Codigo.Contains(searchTerm) || a.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Actividades
            .AnyAsync(a => a.Codigo == codigo && (!excludeId.HasValue || a.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Actividades.CountAsync(a => a.Activo);
    }
}
