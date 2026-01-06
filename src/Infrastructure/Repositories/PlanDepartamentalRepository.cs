using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class PlanDepartamentalRepository : IPlanDepartamentalRepository
{
    private readonly ApplicationDbContext _context;

    public PlanDepartamentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanDepartamental>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.PlanesDepartamentales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.OrderBy(p => p.Codigo).ToListAsync();
    }

    public async Task<PlanDepartamental?> GetByIdAsync(int id)
    {
        return await _context.PlanesDepartamentales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PlanDepartamental> CreateAsync(PlanDepartamental planDepartamental)
    {
        _context.PlanesDepartamentales.Add(planDepartamental);
        await _context.SaveChangesAsync();
        return planDepartamental;
    }

    public async Task UpdateAsync(PlanDepartamental planDepartamental)
    {
        _context.PlanesDepartamentales.Update(planDepartamental);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var planDepartamental = await _context.PlanesDepartamentales.FirstOrDefaultAsync(p => p.Id == id);
            
        if (planDepartamental != null)
        {
            // Soft delete
            planDepartamental.IsDeleted = true;
            planDepartamental.DeletedAt = DateTime.UtcNow;
            planDepartamental.DeletedBy = deletedBy;
            planDepartamental.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PlanDepartamental>> SearchAsync(string searchTerm)
    {
        return await _context.PlanesDepartamentales
            .Include(p => p.Alcaldia)
            .Include(p => p.Sector)
            .Where(p => p.Codigo.Contains(searchTerm) || p.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.PlanesDepartamentales
            .AnyAsync(p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.PlanesDepartamentales.CountAsync(p => p.Activo);
    }
}
