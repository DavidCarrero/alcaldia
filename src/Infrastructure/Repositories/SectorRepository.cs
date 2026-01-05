using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class SectorRepository : ISectorRepository
{
    private readonly ApplicationDbContext _context;

    public SectorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sector>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Sectores
            .Include(s => s.Alcaldia)
            .Include(s => s.LineaEstrategica)
            .Include(s => s.Programas)
            .Include(s => s.PlanesNacionales)
            .Include(s => s.PlanesDepartamentales)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(s => s.Activo);
        }

        return await query.OrderBy(s => s.Codigo).ToListAsync();
    }

    public async Task<Sector?> GetByIdAsync(int id)
    {
        return await _context.Sectores
            .Include(s => s.Alcaldia)
            .Include(s => s.LineaEstrategica)
            .Include(s => s.Programas)
            .Include(s => s.PlanesNacionales)
            .Include(s => s.PlanesDepartamentales)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sector> CreateAsync(Sector sector)
    {
        _context.Sectores.Add(sector);
        await _context.SaveChangesAsync();
        return sector;
    }

    public async Task UpdateAsync(Sector sector)
    {
        _context.Sectores.Update(sector);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sector = await GetByIdAsync(id);
        if (sector != null)
        {
            sector.Activo = false;
            await UpdateAsync(sector);
        }
    }

    public async Task<IEnumerable<Sector>> SearchAsync(string searchTerm)
    {
        return await _context.Sectores
            .Include(s => s.Alcaldia)
            .Include(s => s.LineaEstrategica)
            .Include(s => s.Programas)
            .Include(s => s.PlanesNacionales)
            .Include(s => s.PlanesDepartamentales)
            .Where(s => s.Codigo.Contains(searchTerm) || s.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Sectores
            .AnyAsync(s => s.Codigo == codigo && (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Sectores.CountAsync(s => s.Activo);
    }
}
