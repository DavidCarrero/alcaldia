using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ProgramaRepository : IProgramaRepository
{
    private readonly ApplicationDbContext _context;

    public ProgramaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Programa>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Programas
            .Include(p => p.Alcaldia)
            .Include(p => p.PlanMunicipal)
            .Include(p => p.Sector)
            .Include(p => p.ODS)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<Programa?> GetByIdAsync(int id)
    {
        return await _context.Programas
            .Include(p => p.Alcaldia)
            .Include(p => p.PlanMunicipal)
            .Include(p => p.Sector)
            .Include(p => p.ODS)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Programa> CreateAsync(Programa programa)
    {
        _context.Programas.Add(programa);
        await _context.SaveChangesAsync();
        return programa;
    }

    public async Task UpdateAsync(Programa programa)
    {
        _context.Programas.Update(programa);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var programa = await _context.Programas.FirstOrDefaultAsync(p => p.Id == id);
            
        if (programa != null)
        {
            // Soft delete
            programa.IsDeleted = true;
            programa.DeletedAt = DateTime.UtcNow;
            programa.DeletedBy = deletedBy;
            programa.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Programa>> SearchAsync(string searchTerm)
    {
        return await _context.Programas
            .Include(p => p.Alcaldia)
            .Include(p => p.PlanMunicipal)
            .Include(p => p.Sector)
            .Include(p => p.ODS)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Programas
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Programas.CountAsync(p => p.Activo);
    }
}
