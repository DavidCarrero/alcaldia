using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class AlcaldiaRepository : IAlcaldiaRepository
{
    private readonly ApplicationDbContext _context;

    public AlcaldiaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alcaldia>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Alcaldias
            .Include(a => a.Municipio)
                .ThenInclude(m => m.Departamentos)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(a => a.Activo);
        }

        return await query.OrderBy(a => a.Nit).ToListAsync();
    }

    public async Task<Alcaldia?> GetByIdAsync(int id)
    {
        return await _context.Alcaldias
            .Include(a => a.Municipio)
                .ThenInclude(m => m.Departamentos)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Alcaldia> CreateAsync(Alcaldia alcaldia)
    {
        _context.Alcaldias.Add(alcaldia);
        await _context.SaveChangesAsync();
        return alcaldia;
    }

    public async Task UpdateAsync(Alcaldia alcaldia)
    {
        _context.Alcaldias.Update(alcaldia);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var alcaldia = await GetByIdAsync(id);
        if (alcaldia != null)
        {
            alcaldia.Activo = false;
            await UpdateAsync(alcaldia);
        }
    }

    public async Task<IEnumerable<Alcaldia>> SearchAsync(string searchTerm)
    {
        return await _context.Alcaldias
            .Include(a => a.Municipio)
                .ThenInclude(m => m.Departamentos)
            .Where(a => a.Nit.Contains(searchTerm) ||
                       (a.Municipio != null && a.Municipio.Nombre.Contains(searchTerm)) ||
                       (a.Municipio != null && a.Municipio.Departamentos.Any(d => d.Nombre.Contains(searchTerm))))
            .ToListAsync();
    }

    public async Task<bool> NitExistsAsync(string nit, int? excludeId = null)
    {
        return await _context.Alcaldias
            .AnyAsync(a => a.Nit == nit && (!excludeId.HasValue || a.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Alcaldias.CountAsync(a => a.Activo);
    }
}
