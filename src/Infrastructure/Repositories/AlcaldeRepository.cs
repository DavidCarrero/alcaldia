using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class AlcaldeRepository : IAlcaldeRepository
{
    private readonly ApplicationDbContext _context;

    public AlcaldeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alcalde>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Alcaldes
            .Include(a => a.Alcaldia)
                .ThenInclude(alc => alc.Municipio)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(a => a.Activo);
        }

        return await query
            .OrderByDescending(a => a.PeriodoInicio)
            .ToListAsync();
    }

    public async Task<Alcalde?> GetByIdAsync(int id)
    {
        return await _context.Alcaldes
            .Include(a => a.Alcaldia)
                .ThenInclude(alc => alc.Municipio)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Alcalde> CreateAsync(Alcalde alcalde)
    {
        alcalde.FechaCreacion = DateTime.UtcNow;
        _context.Alcaldes.Add(alcalde);
        await _context.SaveChangesAsync();
        return alcalde;
    }

    public async Task UpdateAsync(Alcalde alcalde)
    {
        alcalde.FechaActualizacion = DateTime.UtcNow;
        _context.Alcaldes.Update(alcalde);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var alcalde = await _context.Alcaldes
            .FirstOrDefaultAsync(a => a.Id == id);
            
        if (alcalde != null)
        {
            // Soft delete
            alcalde.IsDeleted = true;
            alcalde.DeletedAt = DateTime.UtcNow;
            alcalde.DeletedBy = deletedBy;
            alcalde.Activo = false;
            alcalde.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Alcaldes.AnyAsync(a => a.Id == id);
    }

    public async Task<int> GetTotalActivosAsync()
    {
        return await _context.Alcaldes.CountAsync(a => a.Activo);
    }

    public async Task<int> GetTotalEnPeriodoAsync()
    {
        var fechaActual = DateTime.UtcNow.Date;
        return await _context.Alcaldes
            .CountAsync(a => a.Activo && 
                           a.PeriodoInicio <= fechaActual && 
                           (!a.PeriodoFin.HasValue || a.PeriodoFin >= fechaActual));
    }

    public async Task<int> GetTotalPartidosAsync()
    {
        return await _context.Alcaldes
            .Where(a => a.Activo && !string.IsNullOrEmpty(a.PartidoPolitico))
            .Select(a => a.PartidoPolitico)
            .Distinct()
            .CountAsync();
    }
}
