using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class SubsecretariaRepository : ISubsecretariaRepository
{
    private readonly ApplicationDbContext _context;

    public SubsecretariaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subsecretaria>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Subsecretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
            .Include(s => s.SubsecretariasResponsables)
                .ThenInclude(sr => sr.Responsable)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(s => s.Activo);
        }

        return await query
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<Subsecretaria?> GetByIdAsync(int id)
    {
        return await _context.Subsecretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
            .Include(s => s.SubsecretariasResponsables)
                .ThenInclude(sr => sr.Responsable)
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Subsecretaria> CreateAsync(Subsecretaria subsecretaria)
    {
        subsecretaria.FechaCreacion = DateTime.UtcNow;
        _context.Subsecretarias.Add(subsecretaria);
        await _context.SaveChangesAsync();
        return subsecretaria;
    }

    public async Task UpdateAsync(Subsecretaria subsecretaria)
    {
        subsecretaria.FechaActualizacion = DateTime.UtcNow;
        _context.Subsecretarias.Update(subsecretaria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var subsecretaria = await _context.Subsecretarias.FirstOrDefaultAsync(s => s.Id == id);
            
        if (subsecretaria != null)
        {
            // Soft delete
            subsecretaria.IsDeleted = true;
            subsecretaria.DeletedAt = DateTime.UtcNow;
            subsecretaria.DeletedBy = deletedBy;
            subsecretaria.Activo = false;
            subsecretaria.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Subsecretarias.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Subsecretaria>> GetBySecretariaIdAsync(int secretariaId)
    {
        return await _context.Subsecretarias
            .Include(s => s.Alcaldia)
            .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
            .Where(s => !s.IsDeleted && 
                       s.SecretariasSubsecretarias.Any(ss => ss.SecretariaId == secretariaId && !ss.IsDeleted) && 
                       s.Activo)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subsecretaria>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        return await _context.Subsecretarias
            .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
            .Where(s => !s.IsDeleted && s.AlcaldiaId == alcaldiaId && s.Activo)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<int> GetTotalActivasAsync()
    {
        return await _context.Subsecretarias.CountAsync(s => s.Activo);
    }

    public async Task<IEnumerable<Subsecretaria>> SearchAsync(string searchTerm)
    {
        return await _context.Subsecretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
            .Where(s => !s.IsDeleted && 
                       (s.Nombre.Contains(searchTerm) || s.Codigo.Contains(searchTerm) ||
                       (s.Descripcion != null && s.Descripcion.Contains(searchTerm))))
            .ToListAsync();
    }
}
