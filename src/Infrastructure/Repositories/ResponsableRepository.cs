using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ResponsableRepository : IResponsableRepository
{
    private readonly ApplicationDbContext _context;

    public ResponsableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Responsable>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Responsables
            .Include(r => r.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(r => r.Secretaria)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(r => r.Activo);
        }

        return await query
            .OrderBy(r => r.NombreCompleto)
            .ToListAsync();
    }

    public async Task<Responsable?> GetByIdAsync(int id)
    {
        return await _context.Responsables
            .Include(r => r.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(r => r.Secretaria)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Responsable> CreateAsync(Responsable responsable)
    {
        responsable.FechaCreacion = DateTime.UtcNow;
        _context.Responsables.Add(responsable);
        await _context.SaveChangesAsync();
        return responsable;
    }

    public async Task UpdateAsync(Responsable responsable)
    {
        responsable.FechaActualizacion = DateTime.UtcNow;
        _context.Responsables.Update(responsable);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var responsable = await _context.Responsables.FindAsync(id);
        if (responsable != null)
        {
            responsable.Activo = false;
            responsable.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Responsables.AnyAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Responsable>> GetBySecretariaIdAsync(int secretariaId)
    {
        return await _context.Responsables
            .Include(r => r.Alcaldia)
            .Include(r => r.Secretaria)
            .Where(r => r.SecretariaId == secretariaId && r.Activo)
            .OrderBy(r => r.NombreCompleto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Responsable>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        return await _context.Responsables
            .Include(r => r.Secretaria)
            .Where(r => r.AlcaldiaId == alcaldiaId && r.Activo)
            .OrderBy(r => r.NombreCompleto)
            .ToListAsync();
    }

    public async Task<int> GetTotalActivosAsync()
    {
        return await _context.Responsables.CountAsync(r => r.Activo);
    }
}
