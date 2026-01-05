using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class IndicadorRepository : IIndicadorRepository
{
    private readonly ApplicationDbContext _context;

    public IndicadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Indicador>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Indicadores
            .Include(i => i.Alcaldia)
            .Include(i => i.Responsable)
            .Include(i => i.Producto)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(i => i.Activo);
        }

        return await query.OrderBy(i => i.Codigo).ToListAsync();
    }

    public async Task<Indicador?> GetByIdAsync(int id)
    {
        return await _context.Indicadores
            .Include(i => i.Alcaldia)
            .Include(i => i.Responsable)
            .Include(i => i.Producto)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Indicador> CreateAsync(Indicador indicador)
    {
        _context.Indicadores.Add(indicador);
        await _context.SaveChangesAsync();
        return indicador;
    }

    public async Task UpdateAsync(Indicador indicador)
    {
        _context.Indicadores.Update(indicador);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var indicador = await GetByIdAsync(id);
        if (indicador != null)
        {
            indicador.Activo = false;
            await UpdateAsync(indicador);
        }
    }

    public async Task<IEnumerable<Indicador>> SearchAsync(string searchTerm)
    {
        return await _context.Indicadores
            .Include(i => i.Alcaldia)
            .Include(i => i.Responsable)
            .Include(i => i.Producto)
            .Where(i => i.Codigo.Contains(searchTerm) || i.Nombre.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Indicadores
            .AnyAsync(i => i.Codigo == codigo && (!excludeId.HasValue || i.Id != excludeId.Value));
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.Indicadores.CountAsync(i => i.Activo);
    }
}
