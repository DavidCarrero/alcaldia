using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class SecretariaService : ISecretariaService
{
    private readonly ISecretariaRepository _secretariaRepository;
    private readonly ILogger<SecretariaService> _logger;

    public SecretariaService(ISecretariaRepository secretariaRepository, ILogger<SecretariaService> logger)
    {
        _secretariaRepository = secretariaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SecretariaViewModel>> GetAllSecretariasAsync(bool incluirInactivas = false)
    {
        var secretarias = await _secretariaRepository.GetAllAsync(incluirInactivas);
        return secretarias.Select(MapToViewModel);
    }

    public async Task<SecretariaViewModel?> GetSecretariaByIdAsync(int id)
    {
        var secretaria = await _secretariaRepository.GetByIdAsync(id);
        return secretaria != null ? MapToViewModel(secretaria) : null;
    }

    public async Task<SecretariaViewModel> CreateSecretariaAsync(SecretariaViewModel model)
    {
        var secretaria = new Secretaria
        {
            Codigo = model.Codigo,
            Nombre = model.Nombre,
            Sigla = model.Sigla,
            Secretario = model.Secretario,
            Descripcion = model.Descripcion,
            Telefono = model.Telefono,
            CorreoInstitucional = model.CorreoInstitucional,
            PresupuestoAsignado = model.PresupuestoAsignado,
            AlcaldiaId = model.AlcaldiaId,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _secretariaRepository.CreateAsync(secretaria);
        _logger.LogInformation($"Secretaría creada: {secretaria.Nombre}");

        return MapToViewModel(secretaria);
    }

    public async Task UpdateSecretariaAsync(int id, SecretariaViewModel model)
    {
        var secretaria = await _secretariaRepository.GetByIdAsync(id);
        if (secretaria == null)
        {
            throw new InvalidOperationException("Secretaría no encontrada");
        }

        secretaria.Codigo = model.Codigo;
        secretaria.Nombre = model.Nombre;
        secretaria.Sigla = model.Sigla;
        secretaria.Secretario = model.Secretario;
        secretaria.Descripcion = model.Descripcion;
        secretaria.Telefono = model.Telefono;
        secretaria.CorreoInstitucional = model.CorreoInstitucional;
        secretaria.PresupuestoAsignado = model.PresupuestoAsignado;
        secretaria.AlcaldiaId = model.AlcaldiaId;
        secretaria.Activo = model.Activo;
        secretaria.FechaActualizacion = DateTime.UtcNow;

        await _secretariaRepository.UpdateAsync(secretaria);
        _logger.LogInformation($"Secretaría actualizada: {secretaria.Nombre}");
    }

    public async Task DeleteSecretariaAsync(int id)
    {
        await _secretariaRepository.DeleteAsync(id);
        _logger.LogInformation($"Secretaría desactivada: ID {id}");
    }

    public async Task<IEnumerable<SecretariaViewModel>> SearchSecretariasAsync(string searchTerm)
    {
        var secretarias = await _secretariaRepository.SearchAsync(searchTerm);
        return secretarias.Select(MapToViewModel);
    }

    public async Task<IEnumerable<SecretariaViewModel>> GetByAlcaldiaIdAsync(int alcaldiaId)
    {
        var secretarias = await _secretariaRepository.GetByAlcaldiaIdAsync(alcaldiaId);
        return secretarias.Select(MapToViewModel);
    }

    private static SecretariaViewModel MapToViewModel(Secretaria secretaria)
    {
        return new SecretariaViewModel
        {
            Id = secretaria.Id,
            Codigo = secretaria.Codigo,
            Nombre = secretaria.Nombre,
            Sigla = secretaria.Sigla,
            Secretario = secretaria.Secretario,
            Descripcion = secretaria.Descripcion,
            Telefono = secretaria.Telefono,
            CorreoInstitucional = secretaria.CorreoInstitucional,
            PresupuestoAsignado = secretaria.PresupuestoAsignado,
            AlcaldiaId = secretaria.AlcaldiaId,
            NitAlcaldia = secretaria.Alcaldia?.Nit,
            Activo = secretaria.Activo,
            CantidadSubsecretarias = secretaria.Subsecretarias?.Count ?? 0
        };
    }
}
