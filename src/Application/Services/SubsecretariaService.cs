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

    public async Task DeleteAsync(int id, string deletedBy)
    {
        try
        {
            await _subsecretariaRepository.DeleteAsync(id, deletedBy);
            _logger.LogInformation("Subsecretaría eliminada exitosamente con ID {Id} por usuario {DeletedBy}", id, deletedBy);
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
            var conResponsable = subsecretarias.Count(s => s.SubsecretariasResponsables.Any(sr => !sr.IsDeleted));
            var secretarias = subsecretarias
                .SelectMany(s => s.SecretariasSubsecretarias.Select(ss => ss.SecretariaId))
                .Distinct()
                .Count();

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

    public async Task<IEnumerable<SubsecretariaViewModel>> SearchAsync(string searchTerm)
    {
        try
        {
            var subsecretarias = await _subsecretariaRepository.SearchAsync(searchTerm);
            return subsecretarias.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar subsecretarías con término: {SearchTerm}", searchTerm);
            throw;
        }
    }

    private SubsecretariaViewModel MapToViewModel(Subsecretaria subsecretaria)
    {
        // Obtener todas las secretarías asociadas
        var secretarias = subsecretaria.SecretariasSubsecretarias
            .Where(ss => !ss.IsDeleted)
            .Select(ss => ss.Secretaria)
            .ToList();
        
        // Obtener todos los responsables asociados
        var responsables = subsecretaria.SubsecretariasResponsables
            .Where(sr => !sr.IsDeleted)
            .Select(sr => sr.Responsable)
            .ToList();
        
        var primeraSecretaria = secretarias.FirstOrDefault();

        return new SubsecretariaViewModel
        {
            Id = subsecretaria.Id,
            AlcaldiaId = subsecretaria.AlcaldiaId,
            Codigo = subsecretaria.Codigo,
            Nombre = subsecretaria.Nombre,
            Descripcion = subsecretaria.Descripcion,
            SecretariaId = primeraSecretaria?.Id,
            Responsable = subsecretaria.Responsable,
            CorreoInstitucional = subsecretaria.CorreoInstitucional,
            Telefono = subsecretaria.Telefono,
            PresupuestoAsignado = subsecretaria.PresupuestoAsignado,
            Activo = subsecretaria.Activo,
            NitAlcaldia = subsecretaria.Alcaldia?.Nit,
            MunicipioAlcaldia = subsecretaria.Alcaldia?.Municipio?.Nombre,
            CodigoSecretaria = primeraSecretaria?.Codigo,
            NombreSecretaria = primeraSecretaria?.Nombre,
            SecretariasIds = secretarias.Select(s => s.Id).ToList(),
            SecretariasNombres = secretarias.Select(s => s.Nombre).ToList(),
            ResponsablesIds = responsables.Select(r => r.Id).ToList(),
            ResponsablesNombres = responsables.Select(r => r.NombreCompleto).ToList()
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
            Responsable = viewModel.Responsable,
            CorreoInstitucional = viewModel.CorreoInstitucional,
            Telefono = viewModel.Telefono,
            PresupuestoAsignado = viewModel.PresupuestoAsignado,
            Activo = viewModel.Activo
        };
    }
}
