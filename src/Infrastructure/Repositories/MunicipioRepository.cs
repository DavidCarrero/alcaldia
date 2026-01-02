using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Repositories;

public class MunicipioRepository : IMunicipioRepository
{
    private readonly ApplicationDbContext _context;

    public MunicipioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Municipio>> GetAllAsync(bool incluirInactivos = false)
    {
        var query = _context.Municipios
            .Include(m => m.Departamentos)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(m => m.Activo);
        }

        return await query.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<Municipio?> GetByIdAsync(int id)
    {
        return await _context.Municipios
            .Include(m => m.Departamentos)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Municipio> CreateAsync(Municipio municipio, List<int>? departamentoIds = null)
    {
        // Primero guardar el municipio para obtener su ID
        _context.Municipios.Add(municipio);
        await _context.SaveChangesAsync();
        
        // Luego agregar las relaciones con departamentos
        if (departamentoIds != null && departamentoIds.Any())
        {
            var departamentos = await _context.Departamentos
                .Where(d => departamentoIds.Contains(d.Id))
                .ToListAsync();
            
            foreach (var departamento in departamentos)
            {
                municipio.Departamentos.Add(departamento);
            }
            
            await _context.SaveChangesAsync();
        }
        
        return municipio;
    }

    public async Task UpdateAsync(Municipio municipio, List<int>? departamentoIds = null)
    {
        // Cargar el municipio con sus departamentos actuales
        var municipioExistente = await _context.Municipios
            .Include(m => m.Departamentos)
            .FirstOrDefaultAsync(m => m.Id == municipio.Id);
            
        if (municipioExistente != null)
        {
            // Actualizar propiedades básicas
            municipioExistente.Codigo = municipio.Codigo;
            municipioExistente.Nombre = municipio.Nombre;
            municipioExistente.Activo = municipio.Activo;
            municipioExistente.FechaActualizacion = municipio.FechaActualizacion;
            
            // Actualizar relaciones con departamentos
            if (departamentoIds != null)
            {
                // Limpiar departamentos existentes
                municipioExistente.Departamentos.Clear();
                
                // Agregar nuevos departamentos
                if (departamentoIds.Any())
                {
                    var departamentos = await _context.Departamentos
                        .Where(d => departamentoIds.Contains(d.Id))
                        .ToListAsync();
                    
                    foreach (var departamento in departamentos)
                    {
                        municipioExistente.Departamentos.Add(departamento);
                    }
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var municipio = await GetByIdAsync(id);
        if (municipio != null)
        {
            municipio.Activo = false;
            await UpdateAsync(municipio);
        }
    }

    public async Task<IEnumerable<Municipio>> SearchAsync(string searchTerm)
    {
        return await _context.Municipios
            .Include(m => m.Departamentos)
            .Where(m => m.Nombre.Contains(searchTerm) || 
                       m.Codigo.Contains(searchTerm) ||
                       m.Departamentos.Any(d => d.Nombre.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        return await _context.Municipios
            .AnyAsync(m => m.Codigo == codigo && (!excludeId.HasValue || m.Id != excludeId.Value));
    }

    public async Task<IEnumerable<Municipio>> GetByDepartamentoIdAsync(int departamentoId)
    {
        return await _context.Municipios
            .Include(m => m.Departamentos)
            .Where(m => m.Departamentos.Any(d => d.Id == departamentoId) && m.Activo)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }
}
