using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Usuarios
            .Where(u => !u.IsDeleted)
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(u => u.Activo);
        }

        return await query.OrderByDescending(u => u.FechaCreacion).ToListAsync();
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.CorreoElectronico == email);
    }

    public async Task<Usuario?> GetByUsernameAsync(string username)
    {
        return await _context.Usuarios
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == username);
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        usuario.FechaActualizacion = DateTime.UtcNow;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario != null)
        {
            // Soft delete
            usuario.IsDeleted = true;
            usuario.DeletedAt = DateTime.UtcNow;
            usuario.DeletedBy = deletedBy;
            usuario.Activo = false;
            usuario.FechaActualizacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        var query = _context.Usuarios.Where(u => u.CorreoElectronico == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        var query = _context.Usuarios.Where(u => u.NombreUsuario == username);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Usuario>> SearchAsync(string searchTerm)
    {
        return await _context.Usuarios
            .Include(u => u.UsuariosRoles)
                .ThenInclude(ur => ur.Rol)
            .Where(u => u.NombreCompleto.Contains(searchTerm) ||
                       u.CorreoElectronico.Contains(searchTerm) ||
                       (u.NombreUsuario != null && u.NombreUsuario.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<int> CountActiveUsersAsync()
    {
        return await _context.Usuarios.CountAsync(u => u.Activo);
    }

    public async Task<int> CountInactiveUsersAsync()
    {
        return await _context.Usuarios.CountAsync(u => !u.Activo);
    }
}
