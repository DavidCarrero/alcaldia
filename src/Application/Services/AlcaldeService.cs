using Microsoft.Extensions.Logging;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class AlcaldeService : IAlcaldeService
{
    private readonly IAlcaldeRepository _alcaldeRepository;
    private readonly ILogger<AlcaldeService> _logger;

    public AlcaldeService(
        IAlcaldeRepository alcaldeRepository,
        ILogger<AlcaldeService> logger)
    {
        _alcaldeRepository = alcaldeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlcaldeViewModel>> GetAllAsync(bool incluirInactivos = false)
    {
        try
        {
            var alcaldes = await _alcaldeRepository.GetAllAsync(incluirInactivos);
            return alcaldes.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de alcaldes");
            throw;
        }
    }

    public async Task<AlcaldeViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var alcalde = await _alcaldeRepository.GetByIdAsync(id);
            return alcalde != null ? MapToViewModel(alcalde) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el alcalde con ID {Id}", id);
            throw;
        }
    }

    public async Task<AlcaldeViewModel> CreateAsync(AlcaldeViewModel viewModel)
    {
        try
        {
            var alcalde = MapToEntity(viewModel);
            var createdAlcalde = await _alcaldeRepository.CreateAsync(alcalde);
            _logger.LogInformation("Alcalde creado exitosamente: {NombreCompleto}", viewModel.NombreCompleto);
            return MapToViewModel(createdAlcalde);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el alcalde");
            throw;
        }
    }

    public async Task UpdateAsync(AlcaldeViewModel viewModel)
    {
        try
        {
            var alcalde = MapToEntity(viewModel);
            await _alcaldeRepository.UpdateAsync(alcalde);
            _logger.LogInformation("Alcalde actualizado exitosamente: {NombreCompleto}", viewModel.NombreCompleto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el alcalde con ID {Id}", viewModel.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, string deletedBy)
    {
        try
        {
            await _alcaldeRepository.DeleteAsync(id, deletedBy);
            _logger.LogInformation("Alcalde eliminado exitosamente con ID {Id} por usuario {DeletedBy}", id, deletedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el alcalde con ID {Id}", id);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetEstadisticasAsync()
    {
        try
        {
            var totalActivos = await _alcaldeRepository.GetTotalActivosAsync();
            var totalEnPeriodo = await _alcaldeRepository.GetTotalEnPeriodoAsync();
            var totalPartidos = await _alcaldeRepository.GetTotalPartidosAsync();

            return new Dictionary<string, int>
            {
                { "TotalActivos", totalActivos },
                { "EnPeriodo", totalEnPeriodo },
                { "Partidos", totalPartidos }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las estadísticas de alcaldes");
            throw;
        }
    }

    private AlcaldeViewModel MapToViewModel(Alcalde alcalde)
    {
        return new AlcaldeViewModel
        {
            Id = alcalde.Id,
            AlcaldiaId = alcalde.AlcaldiaId,
            NombreCompleto = alcalde.NombreCompleto,
            TipoDocumento = alcalde.TipoDocumento,
            NumeroDocumento = alcalde.NumeroDocumento,
            PeriodoInicio = alcalde.PeriodoInicio,
            PeriodoFin = alcalde.PeriodoFin,
            Slogan = alcalde.Slogan,
            PartidoPolitico = alcalde.PartidoPolitico,
            CorreoElectronico = alcalde.CorreoElectronico,
            Activo = alcalde.Activo,
            NitAlcaldia = alcalde.Alcaldia?.Nit,
            MunicipioAlcaldia = alcalde.Alcaldia?.Municipio?.Nombre
        };
    }

    private Alcalde MapToEntity(AlcaldeViewModel viewModel)
    {
        return new Alcalde
        {
            Id = viewModel.Id,
            AlcaldiaId = viewModel.AlcaldiaId,
            NombreCompleto = viewModel.NombreCompleto,
            TipoDocumento = viewModel.TipoDocumento,
            NumeroDocumento = viewModel.NumeroDocumento,
            PeriodoInicio = viewModel.PeriodoInicio.HasValue 
                ? DateTime.SpecifyKind(viewModel.PeriodoInicio.Value, DateTimeKind.Utc) 
                : null,
            PeriodoFin = viewModel.PeriodoFin.HasValue 
                ? DateTime.SpecifyKind(viewModel.PeriodoFin.Value, DateTimeKind.Utc) 
                : null,
            Slogan = viewModel.Slogan,
            PartidoPolitico = viewModel.PartidoPolitico,
            CorreoElectronico = viewModel.CorreoElectronico,
            Activo = viewModel.Activo
        };
    }
}
