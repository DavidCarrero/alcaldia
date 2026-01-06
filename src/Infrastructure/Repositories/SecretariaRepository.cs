using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;
using System.Text.Json;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class SecretariaRepository : ISecretariaRepository
{
    private readonly ApplicationDbContext _context;

    public SecretariaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Secretaria>> GetAllAsync(bool incluirInactivas = false)
    {
        var query = _context.Secretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias.Where(ss => !ss.IsDeleted))
                .ThenInclude(ss => ss.Subsecretaria)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        if (!incluirInactivas)
        {
            query = query.Where(s => s.Activo);
        }

        return await query.OrderBy(s => s.Nombre).ToListAsync();
    }

    public async Task<Secretaria?> GetByIdAsync(int id)
    {
        return await _context.Secretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias.Where(ss => !ss.IsDeleted))
                .ThenInclude(ss => ss.Subsecretaria)
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Secretaria> CreateAsync(Secretaria secretaria)
    {
        _context.Secretarias.Add(secretaria);
        await _context.SaveChangesAsync();
        return secretaria;
    }

    public async Task UpdateAsync(Secretaria secretaria)
    {
        _context.Secretarias.Update(secretaria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var secretaria = await _context.Secretarias
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (secretaria != null)
        {
            // Soft delete
            secretaria.IsDeleted = true;
            secretaria.DeletedAt = DateTime.UtcNow;
            secretaria.DeletedBy = deletedBy;
            secretaria.Activo = false;
            secretaria.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Secretaria>> SearchAsync(string searchTerm)
    {
        return await _context.Secretarias
            .Include(s => s.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .Include(s => s.SecretariasSubsecretarias.Where(ss => !ss.IsDeleted))
                .ThenInclude(ss => ss.Subsecretaria)
            .Where(s => !s.IsDeleted && 
                       (s.Nombre.Contains(searchTerm) ||
                       (s.Descripcion != null && s.Descripcion.Contains(searchTerm))))
            .ToListAsync();
    }

    public async Task<IEnumerable<Secretaria>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        return await _context.Secretarias
            .Include(s => s.Alcaldia)
            .Include(s => s.SecretariasSubsecretarias.Where(ss => !ss.IsDeleted))
                .ThenInclude(ss => ss.Subsecretaria)
            .Where(s => !s.IsDeleted && s.AlcaldiaId == alcaldiaId && s.Activo)
            .OrderBy(s => s.Nombre)
            .ToListAsync();
    }
}