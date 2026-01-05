using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class MetaODSService : IMetaODSService
{
    private readonly IMetaODSRepository _metaODSRepository;

    public MetaODSService(IMetaODSRepository metaODSRepository)
    {
        _metaODSRepository = metaODSRepository;
    }

    public async Task<IEnumerable<MetaODSViewModel>> GetAllMetasODSAsync(bool incluirInactivas = false)
    {
        var metas = await _metaODSRepository.GetAllAsync(incluirInactivas);
        return metas.Select(MapToViewModel);
    }

    public async Task<MetaODSViewModel?> GetMetaODSByIdAsync(int id)
    {
        var meta = await _metaODSRepository.GetByIdAsync(id);
        return meta != null ? MapToViewModel(meta) : null;
    }

    public async Task<MetaODSViewModel> CreateMetaODSAsync(MetaODSViewModel model)
    {
        if (await _metaODSRepository.CodigoExistsAsync(model.Codigo, model.AlcaldiaId))
        {
            throw new InvalidOperationException("Ya existe una Meta ODS con ese código en esta alcaldía");
        }

        var metaODS = new MetaODS
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        var created = await _metaODSRepository.CreateAsync(metaODS);
        return MapToViewModel(created);
    }

    public async Task UpdateMetaODSAsync(int id, MetaODSViewModel model)
    {
        var metaODS = await _metaODSRepository.GetByIdAsync(id);
        if (metaODS == null)
        {
            throw new InvalidOperationException("Meta ODS no encontrada");
        }

        if (await _metaODSRepository.CodigoExistsAsync(model.Codigo, model.AlcaldiaId, id))
        {
            throw new InvalidOperationException("Ya existe una Meta ODS con ese código en esta alcaldía");
        }

        metaODS.AlcaldiaId = model.AlcaldiaId;
        metaODS.Codigo = model.Codigo;
        metaODS.Nombre = model.Nombre;
        metaODS.Descripcion = model.Descripcion;
        metaODS.Activo = model.Activo;
        metaODS.FechaActualizacion = DateTime.UtcNow;

        await _metaODSRepository.UpdateAsync(metaODS);
    }

    public async Task DeleteMetaODSAsync(int id)
    {
        await _metaODSRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MetaODSViewModel>> SearchMetasODSAsync(string searchTerm)
    {
        var metas = await _metaODSRepository.SearchAsync(searchTerm);
        return metas.Select(MapToViewModel);
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int alcaldiaId, int? excludeId = null)
    {
        return await _metaODSRepository.CodigoExistsAsync(codigo, alcaldiaId, excludeId);
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
