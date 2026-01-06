using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly ApplicationDbContext _context;

    public DepartamentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Departamento>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Departamentos
            .Where(d => !d.IsDeleted)
            .Include(d => d.Municipios)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(d => d.Activo);
        }

        return await query.OrderBy(d => d.Nombre).ToListAsync();
    }

    public async Task<Departamento?> GetByIdAsync(int id)
    {
        return await _context.Departamentos
            .Include(d => d.Municipios)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Departamento> CreateAsync(Departamento departamento)
    {
        _context.Departamentos.Add(departamento);
        await _context.SaveChangesAsync();
        return departamento;
    }

    public async Task UpdateAsync(Departamento departamento)
    {
        _context.Departamentos.Update(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var departamento = await _context.Departamentos.FirstOrDefaultAsync(d => d.Id == id);
        if (departamento != null)
        {
            // Soft delete
            departamento.IsDeleted = true;
            departamento.DeletedAt = DateTime.UtcNow;
            departamento.DeletedBy = deletedBy;
            departamento.Activo = false;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Departamento>> SearchAsync(string searchTerm)
    {
        return await _context.Departamentos
            .Include(d => d.Municipios)
            .Where(d => d.Nombre.Contains(searchTerm) || d.Codigo.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Departamentos
            .AnyAsync(d => d.Codigo == codigo && (!excludeId.HasValue || d.Id != excludeId.Value));
    }
}
