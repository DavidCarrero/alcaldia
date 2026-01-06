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
            .Where(r => !r.IsDeleted) // SIEMPRE excluir eliminados
            .Include(r => r.Alcaldia)
                .ThenInclude(a => a.Municipio)
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
            .Where(r => !r.IsDeleted)
            .Include(r => r.Alcaldia)
                .ThenInclude(a => a.Municipio)
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

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var responsable = await _context.Responsables.FirstOrDefaultAsync(r => r.Id == id);
            
        if (responsable != null)
        {
            // Soft delete
            responsable.IsDeleted = true;
            responsable.DeletedAt = DateTime.UtcNow;
            responsable.DeletedBy = deletedBy;
            responsable.Activo = false;
            responsable.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Responsables.AnyAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Responsable>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        return await _context.Responsables
            .Where(r => !r.IsDeleted)
            .Include(r => r.Alcaldia)
            .Where(r => r.AlcaldiaId == alcaldiaId && r.Activo)
            .OrderBy(r => r.NombreCompleto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Responsable>> SearchAsync(string searchTerm)
    {
        // Intentar parsear el término de búsqueda como número para buscar por ID/Código
        bool isNumeric = int.TryParse(searchTerm, out int searchId);
        
        return await _context.Responsables
            .Where(r => !r.IsDeleted)
            .Include(r => r.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Where(r => 
                (isNumeric && r.Id == searchId) ||
                r.NombreCompleto.Contains(searchTerm) || 
                r.NumeroIdentificacion.Contains(searchTerm) ||
                (r.Cargo != null && r.Cargo.Contains(searchTerm)))
            .OrderBy(r => r.NombreCompleto)
            .ToListAsync();
    }

    public async Task<int> GetTotalActivosAsync()
    {
        return await _context.Responsables.CountAsync(r => !r.IsDeleted && r.Activo);
    }
}
