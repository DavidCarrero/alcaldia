using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class PlanNacionalRepository : IPlanNacionalRepository
{
    private readonly ApplicationDbContext _context;

    public PlanNacionalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanNacional>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.PlanesNacionales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .Include(p => p.PlanesMunicipales)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<PlanNacional?> GetByIdAsync(int id)
    {
        return await _context.PlanesNacionales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .Include(p => p.PlanesMunicipales)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PlanNacional> CreateAsync(PlanNacional planNacional)
    {
        _context.PlanesNacionales.Add(planNacional);
        await _context.SaveChangesAsync();
        return planNacional;
    }

    public async Task UpdateAsync(PlanNacional planNacional)
    {
        _context.PlanesNacionales.Update(planNacional);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var planNacional = await GetByIdAsync(id);
        if (planNacional != null)
        {
            planNacional.Activo = false;
            await UpdateAsync(planNacional);
        }
    }

    public async Task<IEnumerable<PlanNacional>> SearchAsync(string searchTerm)
    {
        return await _context.PlanesNacionales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .Include(p => p.PlanesMunicipales)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.PlanesNacionales
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.PlanesNacionales.CountAsync(p => p.Activo);
    }
}
