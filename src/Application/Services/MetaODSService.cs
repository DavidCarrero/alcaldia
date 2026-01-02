using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Application.Services;

public class MetaODSService : IMetaODSService
{
    private readonly ApplicationDbContext _context;

    public MetaODSService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MetaODSViewModel>> GetAllMetasODSAsync(bool incluirInactivos = false)
    {
        var query = _context.MetasODS
            .Include(m => m.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .AsQueryable();

        if (!incluirInactivos)
        {
            query = query.Where(m => m.Activo);
        }

        var metas = await query.OrderBy(m => m.Codigo).ToListAsync();
        return metas.Select(MapToViewModel);
    }

    public async Task<MetaODSViewModel?> GetMetaODSByIdAsync(int id)
    {
        var meta = await _context.MetasODS
            .Include(m => m.Alcaldia)
                .ThenInclude(a => a.Municipio)
            .FirstOrDefaultAsync(m => m.Id == id);

        return meta != null ? MapToViewModel(meta) : null;
    }

    private static MetaODSViewModel MapToViewModel(MetaODS meta)
    {
        return new MetaODSViewModel
        {
            Id = meta.Id,
            AlcaldiaId = meta.AlcaldiaId,
            NitAlcaldia = meta.Alcaldia?.Nit,
            MunicipioAlcaldia = meta.Alcaldia?.Municipio?.Nombre,
            Codigo = meta.Codigo,
            Nombre = meta.Nombre,
            Descripcion = meta.Descripcion,
            Activo = meta.Activo,
            CantidadODS = meta.ODSMetasODS?.Count ?? 0
        };
    }
}
