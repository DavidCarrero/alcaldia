using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class MetaODSRepository : IMetaODSRepository
{
    private readonly ApplicationDbContext _context;

    public MetaODSRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MetaODS>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.MetasODS
            .Include(m => m.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(m => m.ODSMetasODS)
                .ThenInclude(om => om.ODS)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(m => m.Activo);
        }

        return await query.OrderBy(m => m.Codigo).ToListAsync();
    }

    public async Task<MetaODS?> GetByIdAsync(int id)
    {
        return await _context.MetasODS
            .Include(m => m.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(m => m.ODSMetasODS)
                .ThenInclude(om => om.ODS)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MetaODS> CreateAsync(MetaODS metaODS)
    {
        _context.MetasODS.Add(metaODS);
        await _context.SaveChangesAsync();
        return metaODS;
    }

    public async Task UpdateAsync(MetaODS metaODS)
    {
        _context.MetasODS.Update(metaODS);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var metaODS = await _context.MetasODS.FirstOrDefaultAsync(m => m.Id == id);
            
        if (metaODS != null)
        {
            // Soft delete
            metaODS.IsDeleted = true;
            metaODS.DeletedAt = DateTime.UtcNow;
            metaODS.DeletedBy = deletedBy;
            metaODS.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MetaODS>> SearchAsync(string searchTerm)
    {
        return await _context.MetasODS
            .Include(m => m.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(m => m.ODSMetasODS)
                .ThenInclude(om => om.ODS)
            .Where(m => m.Codigo.Contains(searchTerm) ||
                       m.Nombre.Contains(searchTerm) ||
                       (m.Descripcion != null && m.Descripcion.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null)
    {
        return await _context.MetasODS
            .AnyAsync(m => m.Codigo == codigo && 
                          m.AlcaldiaId == alcaldiaId && 
                          (!excludeId.HasValue || m.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.MetasODS.CountAsync(m => m.Activo);
    }
}
