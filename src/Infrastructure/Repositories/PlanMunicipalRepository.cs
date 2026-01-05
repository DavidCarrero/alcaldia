using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class PlanMunicipalRepository : IPlanMunicipalRepository
{
    private readonly ApplicationDbContext _context;

    public PlanMunicipalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanMunicipal>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.PlanesMunicipales
            .Include(p => p.Alcaldia)
            .Include(p => p.Municipio)
            .Include(p => p.Alcalde)
            .Include(p => p.PlanNacional)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<PlanMunicipal?> GetByIdAsync(int id)
    {
        return await _context.PlanesMunicipales
            .Include(p => p.Alcaldia)
            .Include(p => p.Municipio)
            .Include(p => p.Alcalde)
            .Include(p => p.PlanNacional)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PlanMunicipal> CreateAsync(PlanMunicipal planMunicipal)
    {
        _context.PlanesMunicipales.Add(planMunicipal);
        await _context.SaveChangesAsync();
        return planMunicipal;
    }

    public async Task UpdateAsync(PlanMunicipal planMunicipal)
    {
        _context.PlanesMunicipales.Update(planMunicipal);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var planMunicipal = await GetByIdAsync(id);
        if (planMunicipal != null)
        {
            planMunicipal.Activo = false;
            await UpdateAsync(planMunicipal);
        }
    }

    public async Task<IEnumerable<PlanMunicipal>> SearchAsync(string searchTerm)
    {
        return await _context.PlanesMunicipales
            .Include(p => p.Alcaldia)
            .Include(p => p.Municipio)
            .Include(p => p.Alcalde)
            .Include(p => p.PlanNacional)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.PlanesMunicipales
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.PlanesMunicipales.CountAsync(p => p.Activo);
    }
}
