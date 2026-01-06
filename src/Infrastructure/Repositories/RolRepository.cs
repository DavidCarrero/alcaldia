using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly ApplicationDbContext _context;

    public RolRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rol>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Roles
            .Where(r => !r.IsDeleted)
            .Include(r => r.UsuariosRoles)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(r => r.Activo);
        }

        return await query.OrderBy(r => r.Nombre).ToListAsync();
    }

    public async Task<Rol?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .Include(r => r.UsuariosRoles)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rol?> GetByNameAsync(string nombre)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == nombre);
    }

    public async Task<Rol> CreateAsync(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
        return rol;
    }

    public async Task UpdateAsync(Rol rol)
    {
        rol.FechaActualizacion = DateTime.UtcNow;
        _context.Roles.Update(rol);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (rol != null)
        {
            // Soft delete
            rol.IsDeleted = true;
            rol.DeletedAt = DateTime.UtcNow;
            rol.DeletedBy = deletedBy;
            rol.Activo = false;
            rol.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> NameExistsAsync(string nombre, int? excludeRolId = null)
    {
        var query = _context.Roles.Where(r => r.Nombre == nombre);
        
        if (excludeRolId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRolId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Rol>> SearchAsync(string searchTerm)
    {
        return await _context.Roles
            .Include(r => r.UsuariosRoles)
            .Where(r => r.Nombre.Contains(searchTerm) ||
                       (r.Descripcion != null && r.Descripcion.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<int> CountActiveRolesAsync()
    {
        return await _context.Roles.CountAsync(r => r.Activo);
    }

    public async Task<int> GetUsuariosCountByRolAsync(int rolId)
    {
        return await _context.UsuariosRoles
            .Where(ur => ur.RolId == rolId && ur.Activo)
            .CountAsync();
    }
}
