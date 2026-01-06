using Microsoft.Extensions.Logging;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class ResponsableService : IResponsableService
{
    private readonly IResponsableRepository _responsableRepository;
    private readonly ILogger<ResponsableService> _logger;

    public ResponsableService(
        IResponsableRepository responsableRepository,
        ILogger<ResponsableService> logger)
    {
        _responsableRepository = responsableRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ResponsableViewModel>> GetAllAsync(bool incluirInactivos = false)
    {
        try
        {
            var responsables = await _responsableRepository.GetAllAsync(incluirInactivos);
            return responsables.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de responsables");
            throw;
        }
    }

    public async Task<ResponsableViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var responsable = await _responsableRepository.GetByIdAsync(id);
            return responsable != null ? MapToViewModel(responsable) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el responsable con ID {Id}", id);
            throw;
        }
    }

    public async Task<ResponsableViewModel> CreateAsync(ResponsableViewModel viewModel)
    {
        try
        {
            var responsable = MapToEntity(viewModel);
            var createdResponsable = await _responsableRepository.CreateAsync(responsable);
            _logger.LogInformation("Responsable creado exitosamente: {NombreCompleto}", viewModel.NombreCompleto);
            return MapToViewModel(createdResponsable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el responsable");
            throw;
        }
    }

    public async Task UpdateAsync(ResponsableViewModel viewModel)
    {
        try
        {
            var responsable = MapToEntity(viewModel);
            await _responsableRepository.UpdateAsync(responsable);
            _logger.LogInformation("Responsable actualizado exitosamente: {NombreCompleto}", viewModel.NombreCompleto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el responsable con ID {Id}", viewModel.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        try
        {
            await _responsableRepository.DeleteAsync(id, deletedBy);
            _logger.LogInformation("Responsable eliminado exitosamente con ID {Id} por usuario {DeletedBy}", id, deletedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el responsable con ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ResponsableViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        try
        {
            var responsables = await _responsableRepository.GetByAlcaldiaIdAsync(alcaldiaId);
            return responsables.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener responsables de la alcaldía {AlcaldiaId}", alcaldiaId);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetEstadisticasAsync()
    {
        try
        {
            var totalActivos = await _responsableRepository.GetTotalActivosAsync();
            var responsables = await _responsableRepository.GetAllAsync();
            var conSubsecretaria = responsables.Count(r => r.SubsecretariasResponsables.Any(sr => !sr.IsDeleted));
            var tiposResponsable = responsables
                .Where(r => !string.IsNullOrEmpty(r.TipoResponsable))
                .Select(r => r.TipoResponsable)
                .Distinct()
                .Count();

            return new Dictionary<string, int>
            {
                { "TotalActivos", totalActivos },
                { "ConSecretaria", conSubsecretaria },
                { "TiposResponsable", tiposResponsable }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las estadísticas de responsables");
            throw;
        }
    }

    private ResponsableViewModel MapToViewModel(Responsable responsable)
    {
        return new ResponsableViewModel
        {
            Id = responsable.Id,
            AlcaldiaId = responsable.AlcaldiaId,
            TipoDocumento = responsable.TipoDocumento,
            NumeroIdentificacion = responsable.NumeroIdentificacion,
            NombreCompleto = responsable.NombreCompleto,
            Telefono = responsable.Telefono,
            Email = responsable.Email,
            Cargo = responsable.Cargo,
            TipoResponsable = responsable.TipoResponsable,
            Activo = responsable.Activo,
            NitAlcaldia = responsable.Alcaldia?.Nit,
            MunicipioAlcaldia = responsable.Alcaldia?.Municipio?.Nombre
        };
    }

    private Responsable MapToEntity(ResponsableViewModel viewModel)
    {
        return new Responsable
        {
            Id = viewModel.Id,
            AlcaldiaId = viewModel.AlcaldiaId,
            TipoDocumento = viewModel.TipoDocumento,
            NumeroIdentificacion = viewModel.NumeroIdentificacion,
            NombreCompleto = viewModel.NombreCompleto,
            Telefono = viewModel.Telefono,
            Email = viewModel.Email,
            Cargo = viewModel.Cargo,
            TipoResponsable = viewModel.TipoResponsable,
            Activo = viewModel.Activo
        };
    }

    public async Task<IEnumerable<ResponsableViewModel>> SearchAsync(string searchTerm)
    {
        try
        {
            var responsables = await _responsableRepository.SearchAsync(searchTerm);
            return responsables.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar responsables con el término: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
