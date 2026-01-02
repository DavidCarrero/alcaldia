using Microsoft.Extensions.Logging;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class SubsecretariaService : ISubsecretariaService
{
    private readonly ISubsecretariaRepository _subsecretariaRepository;
    private readonly ILogger<SubsecretariaService> _logger;

    public SubsecretariaService(
        ISubsecretariaRepository subsecretariaRepository,
        ILogger<SubsecretariaService> logger)
    {
        _subsecretariaRepository = subsecretariaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SubsecretariaViewModel>> GetAllAsync(bool incluirInactivos = false)
    {
        try
        {
            var subsecretarias = await _subsecretariaRepository.GetAllAsync(incluirInactivos);
            return subsecretarias.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de subsecretarías");
            throw;
        }
    }

    public async Task<SubsecretariaViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var subsecretaria = await _subsecretariaRepository.GetByIdAsync(id);
            return subsecretaria != null ? MapToViewModel(subsecretaria) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la subsecretaría con ID {Id}", id);
            throw;
        }
    }

    public async Task<SubsecretariaViewModel> CreateAsync(SubsecretariaViewModel viewModel)
    {
        try
        {
            var subsecretaria = MapToEntity(viewModel);
            var created = await _subsecretariaRepository.CreateAsync(subsecretaria);
            _logger.LogInformation("Subsecretaría creada exitosamente: {Nombre}", viewModel.Nombre);
            return MapToViewModel(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la subsecretaría");
            throw;
        }
    }

    public async Task UpdateAsync(SubsecretariaViewModel viewModel)
    {
        try
        {
            var subsecretaria = MapToEntity(viewModel);
            await _subsecretariaRepository.UpdateAsync(subsecretaria);
            _logger.LogInformation("Subsecretaría actualizada exitosamente: {Nombre}", viewModel.Nombre);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la subsecretaría con ID {Id}", viewModel.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            await _subsecretariaRepository.DeleteAsync(id);
            _logger.LogInformation("Subsecretaría eliminada exitosamente con ID {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la subsecretaría con ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<SubsecretariaViewModel>> GetBySecretariaIdAsync(int secretariaId)
    {
        try
        {
            var subsecretarias = await _subsecretariaRepository.GetBySecretariaIdAsync(secretariaId);
            return subsecretarias.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener subsecretarías de la secretaría {SecretariaId}", secretariaId);
            throw;
        }
    }

    public async Task<IEnumerable<SubsecretariaViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        try
        {
            var subsecretarias = await _subsecretariaRepository.GetByAlcaldiaIdAsync(alcaldiaId);
            return subsecretarias.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener subsecretarías de la alcaldía {AlcaldiaId}", alcaldiaId);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetEstadisticasAsync()
    {
        try
        {
            var totalActivas = await _subsecretariaRepository.GetTotalActivasAsync();
            var subsecretarias = await _subsecretariaRepository.GetAllAsync();
            var conResponsable = subsecretarias.Count(s => !string.IsNullOrEmpty(s.Responsable));
            var secretarias = subsecretarias.Select(s => s.SecretariaId).Distinct().Count();

            return new Dictionary<string, int>
            {
                { "TotalActivas", totalActivas },
                { "ConResponsable", conResponsable },
                { "Secretarias", secretarias }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las estadísticas de subsecretarías");
            throw;
        }
    }

    private SubsecretariaViewModel MapToViewModel(Subsecretaria subsecretaria)
    {
        return new SubsecretariaViewModel
        {
            Id = subsecretaria.Id,
            AlcaldiaId = subsecretaria.AlcaldiaId,
            Codigo = subsecretaria.Codigo,
            Nombre = subsecretaria.Nombre,
            Descripcion = subsecretaria.Descripcion,
            SecretariaId = subsecretaria.SecretariaId,
            Responsable = subsecretaria.Responsable,
            CorreoInstitucional = subsecretaria.CorreoInstitucional,
            Telefono = subsecretaria.Telefono,
            PresupuestoAsignado = subsecretaria.PresupuestoAsignado,
            Activo = subsecretaria.Activo,
            NitAlcaldia = subsecretaria.Alcaldia?.Nit,
            MunicipioAlcaldia = subsecretaria.Alcaldia?.Municipio?.Nombre,
            CodigoSecretaria = subsecretaria.Secretaria?.Codigo,
            NombreSecretaria = subsecretaria.Secretaria?.Nombre
        };
    }

    private Subsecretaria MapToEntity(SubsecretariaViewModel viewModel)
    {
        return new Subsecretaria
        {
            Id = viewModel.Id,
            AlcaldiaId = viewModel.AlcaldiaId,
            Codigo = viewModel.Codigo,
            Nombre = viewModel.Nombre,
            Descripcion = viewModel.Descripcion,
            SecretariaId = viewModel.SecretariaId,
            Responsable = viewModel.Responsable,
            CorreoInstitucional = viewModel.CorreoInstitucional,
            Telefono = viewModel.Telefono,
            PresupuestoAsignado = viewModel.PresupuestoAsignado,
            Activo = viewModel.Activo
        };
    }
}
