using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class ODSRepository : IODSRepository
{
    private readonly ApplicationDbContext _context;

    public ODSRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ODS>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.ODS
            .Include(o => o.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(o => o.ODSMetasODS)
                .ThenInclude(om => om.MetaODS)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(o => o.Activo);
        }

        return await query.OrderBy(o => o.Codigo).ToListAsync();
    }

    public async Task<ODS?> GetByIdAsync(int id)
    {
        return await _context.ODS
            .Include(o => o.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(o => o.ODSMetasODS)
                .ThenInclude(om => om.MetaODS)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<ODS> CreateAsync(ODS ods)
    {
        _context.ODS.Add(ods);
        await _context.SaveChangesAsync();
        return ods;
    }

    public async Task UpdateAsync(ODS ods)
    {
        _context.ODS.Update(ods);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ods = await GetByIdAsync(id);
        if (ods != null)
        {
            ods.Activo = false;
            await UpdateAsync(ods);
        }
    }

    public async Task<IEnumerable<ODS>> SearchAsync(string searchTerm)
    {
        return await _context.ODS
            .Include(o => o.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(o => o.ODSMetasODS)
                .ThenInclude(om => om.MetaODS)
            .Where(o => o.Codigo.Contains(searchTerm) ||
                       o.Nombre.Contains(searchTerm) ||
                       (o.Descripcion != null && o.Descripcion.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null)
    {
        return await _context.ODS
            .AnyAsync(o => o.Codigo == codigo && 
                          o.AlcaldiaId == alcaldiaId && 
                          (!excludeId.HasValue || o.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.ODS.CountAsync(o => o.Activo);
    }
}
