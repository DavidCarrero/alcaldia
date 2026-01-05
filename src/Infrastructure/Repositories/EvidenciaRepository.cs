using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class EvidenciaRepository : IEvidenciaRepository
{
    private readonly ApplicationDbContext _context;

    public EvidenciaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evidencia>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Evidencias
            .Include(e => e.Alcaldia)
            .Include(e => e.Actividad)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(e => e.Activo);
        }

        return await query.OrderBy(e => e.Codigo).ToListAsync();
    }

    public async Task<Evidencia?> GetByIdAsync(int id)
    {
        return await _context.Evidencias
            .Include(e => e.Alcaldia)
            .Include(e => e.Actividad)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evidencia> CreateAsync(Evidencia evidencia)
    {
        _context.Evidencias.Add(evidencia);
        await _context.SaveChangesAsync();
        return evidencia;
    }

    public async Task UpdateAsync(Evidencia evidencia)
    {
        _context.Evidencias.Update(evidencia);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var evidencia = await GetByIdAsync(id);
        if (evidencia != null)
        {
            evidencia.Activo = false;
            await UpdateAsync(evidencia);
        }
    }

    public async Task<IEnumerable<Evidencia>> SearchAsync(string searchTerm)
    {
        return await _context.Evidencias
            .Include(e => e.Alcaldia)
            .Include(e => e.Actividad)
            .Where(e => e.Codigo.Contains(searchTerm) || e.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Evidencias
            .AnyAsync(e => e.Codigo == codigo && (!excludeId.HasValue || e.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Evidencias.CountAsync(e => e.Activo);
    }
}
