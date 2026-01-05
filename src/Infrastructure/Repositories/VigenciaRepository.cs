using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class VigenciaRepository : IVigenciaRepository
{
    private readonly ApplicationDbContext _context;

    public VigenciaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vigencia>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Vigencias
            .Include(v => v.Alcaldia)
            .Include(v => v.AlcaldesVigencias)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(v => v.Activo);
        }

        return await query.OrderBy(v => v.Año).ToListAsync();
    }

    public async Task<Vigencia?> GetByIdAsync(int id)
    {
        return await _context.Vigencias
            .Include(v => v.Alcaldia)
            .Include(v => v.AlcaldesVigencias)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Vigencia> CreateAsync(Vigencia vigencia)
    {
        _context.Vigencias.Add(vigencia);
        await _context.SaveChangesAsync();
        return vigencia;
    }

    public async Task UpdateAsync(Vigencia vigencia)
    {
        _context.Vigencias.Update(vigencia);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var vigencia = await GetByIdAsync(id);
        if (vigencia != null)
        {
            vigencia.Activo = false;
            await UpdateAsync(vigencia);
        }
    }

    public async Task<IEnumerable<Vigencia>> SearchAsync(string searchTerm)
    {
        return await _context.Vigencias
            .Include(v => v.Alcaldia)
            .Include(v => v.AlcaldesVigencias)
            .Where(v => (v.Codigo != null && v.Codigo.Contains(searchTerm)) || v.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Vigencias
            .AnyAsync(v => v.Codigo == codigo && (!excludeId.HasValue || v.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Vigencias.CountAsync(v => v.Activo);
    }
}
