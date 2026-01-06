using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ODSService : IODSService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ODSService> _logger;

    public ODSService(IUnitOfWork unitOfWork, ILogger<ODSService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<ODSViewModel>> GetAllODSAsync(bool incluirInactivos = false)
    {
        var ods = await _unitOfWork.ODS.GetAllAsync(incluirInactivos);
        return ods.Select(MapToViewModel);
    }

    public async Task<ODSViewModel?> GetODSByIdAsync(int id)
    {
        var ods = await _unitOfWork.ODS.GetByIdAsync(id);
        return ods != null ? MapToViewModel(ods) : null;
    }

    public async Task<ODSViewModel> CreateODSAsync(ODSViewModel model)
    {
        var ods = new ODS
        {
            AlcaldiaId = model.AlcaldiaId,
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _unitOfWork.ODS.CreateAsync(ods);
        await _unitOfWork.CompleteAsync();

        // Asociar metas ODS si se proporcionaron
        if (model.MetasODSIds != null && model.MetasODSIds.Any())
        {
            foreach (var metaId in model.MetasODSIds)
            {
                var odsMetaODS = new ODSMetaODS
                {
                    ODSId = ods.Id,
                    MetaODSId = metaId,
                    FechaAsociacion = DateTime.UtcNow
                };
                await _unitOfWork.Context.Set<ODSMetaODS>().AddAsync(odsMetaODS);
            }
            await _unitOfWork.CompleteAsync();
        }

        _logger.LogInformation($"ODS creado: {ods.Codigo}");
        return MapToViewModel(ods);
    }

    public async Task UpdateODSAsync(int id, ODSViewModel model)
    {
        var ods = await _unitOfWork.ODS.GetByIdAsync(id);
        if (ods == null)
            throw new InvalidOperationException("ODS no encontrado");

        ods.Codigo = model.Codigo;
        ods.Nombre = model.Nombre;
        ods.Descripcion = model.Descripcion;
        ods.FechaInicio = model.FechaInicio;
        ods.FechaFin = model.FechaFin;
        ods.Activo = model.Activo;

        await _unitOfWork.ODS.UpdateAsync(ods);

        // Actualizar metas asociadas
        var metasActuales = await _unitOfWork.Context.Set<ODSMetaODS>()
            .Where(om => om.ODSId == id)
            .ToListAsync();

        _unitOfWork.Context.Set<ODSMetaODS>().RemoveRange(metasActuales);

        if (model.MetasODSIds != null && model.MetasODSIds.Any())
        {
            foreach (var metaId in model.MetasODSIds)
            {
                var odsMetaODS = new ODSMetaODS
                {
                    ODSId = ods.Id,
                    MetaODSId = metaId,
                    FechaAsociacion = DateTime.UtcNow
                };
                await _unitOfWork.Context.Set<ODSMetaODS>().AddAsync(odsMetaODS);
            }
        }

        await _unitOfWork.CompleteAsync();
        _logger.LogInformation($"ODS actualizado: {ods.Codigo}");
    }

    public async Task DeleteODSAsync(int id)
    {
        await _unitOfWork.ODS.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        _logger.LogInformation($"ODS eliminado: {id}");
    }

    public async Task<IEnumerable<ODSViewModel>> SearchODSAsync(string searchTerm)
    {
        var ods = await _unitOfWork.ODS.SearchAsync(searchTerm);
        return ods.Select(MapToViewModel);
    }

    private static ODSViewModel MapToViewModel(ODS ods)
    {
        var metasODSIds = ods.ODSMetasODS?.Select(om => om.MetaODSId).ToList() ?? new List<int>();
        
        return new ODSViewModel
        {
            Id = ods.Id,
            AlcaldiaId = ods.AlcaldiaId,
            NitAlcaldia = ods.Alcaldia?.Nit,
            MunicipioAlcaldia = ods.Alcaldia?.Municipio?.Nombre,
            Codigo = ods.Codigo,
            Nombre = ods.Nombre,
            Descripcion = ods.Descripcion,
            FechaInicio = ods.FechaInicio,
            FechaFin = ods.FechaFin,
            Activo = ods.Activo,
            MetasODSIds = metasODSIds
        };
    }
}
