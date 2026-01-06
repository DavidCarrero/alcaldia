using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class RolService : IRolService
{
    private readonly IRolRepository _rolRepository;
    private readonly ILogger<RolService> _logger;

    public RolService(IRolRepository rolRepository, ILogger<RolService> logger)
    {
        _rolRepository = rolRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RolViewModel>> GetAllRolesAsync(bool incluirInactivos = false)
    {
        var roles = await _rolRepository.GetAllAsync(incluirInactivos);
        return roles.Select(MapToViewModel);
    }

    public async Task<RolViewModel?> GetRolByIdAsync(int id)
    {
        var rol = await _rolRepository.GetByIdAsync(id);
        return rol != null ? MapToViewModel(rol) : null;
    }

    public async Task<RolViewModel> CreateRolAsync(RolViewModel model)
    {
        // Validar nombre único
        if (await _rolRepository.NameExistsAsync(model.Nombre))
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre");
        }

        var rol = new Rol
        {
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _rolRepository.CreateAsync(rol);
        _logger.LogInformation($"Rol creado: {rol.Nombre}");

        return MapToViewModel(rol);
    }

    public async Task UpdateRolAsync(int id, RolViewModel model)
    {
        var rol = await _rolRepository.GetByIdAsync(id);
        if (rol == null)
        {
            throw new InvalidOperationException("Rol no encontrado");
        }

        // Validar nombre único
        if (await _rolRepository.NameExistsAsync(model.Nombre, id))
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre");
        }

        rol.Nombre = model.Nombre;
        rol.Descripcion = model.Descripcion;
        rol.Activo = model.Activo;

        await _rolRepository.UpdateAsync(rol);
        _logger.LogInformation($"Rol actualizado: {rol.Nombre}");
    }

    public async Task DeleteRolAsync(int id)
    {
        await _rolRepository.DeleteAsync(id, "Sistema");
        _logger.LogInformation($"Rol desactivado: ID {id}");
    }

    public async Task<IEnumerable<RolViewModel>> SearchRolesAsync(string searchTerm)
    {
        var roles = await _rolRepository.SearchAsync(searchTerm);
        return roles.Select(MapToViewModel);
    }

    public async Task<bool> NameExistsAsync(string nombre, int? excludeRolId = null)
    {
        return await _rolRepository.NameExistsAsync(nombre, excludeRolId);
    }

    public async Task<(int totalRoles, int permisosConfigurados, int usuariosAsignados, int rolesPersonalizados)> GetEstadisticasAsync()
    {
        var roles = await _rolRepository.GetAllAsync(true);
        var totalRoles = roles.Count();
        var rolesActivos = roles.Count(r => r.Activo);
        
        // Contar usuarios totales asignados
        int usuariosAsignados = 0;
        foreach (var rol in roles)
        {
            var count = await _rolRepository.GetUsuariosCountByRolAsync(rol.Id);
            usuariosAsignados += count;
        }

        // Valores simulados para permisos y personalizados
        var permisosConfigurados = rolesActivos * 5; // Aproximado
        var rolesPersonalizados = Math.Max(0, rolesActivos - 4); // Restando roles básicos

        return (totalRoles, permisosConfigurados, usuariosAsignados, rolesPersonalizados);
    }

    private static RolViewModel MapToViewModel(Rol rol)
    {
        var viewModel = new RolViewModel
        {
            Id = rol.Id,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            Activo = rol.Activo,
            Permisos = new Dictionary<string, PermisosModulo>(),
            PermisosModulo = new Dictionary<string, PermisosModulo>()
        };

        // Inicializar permisos por defecto
        var modulos = new[]
        {
            ("PlanDesarrollo", "Plan de Desarrollo Municipal"),
            ("Hacienda", "Hacienda y Presupuesto"),
            ("Impuestos", "Impuestos y Recaudos"),
            ("Proyectos", "Proyectos y Obras"),
            ("Usuarios", "Gestión de Usuarios")
        };

        foreach (var (key, nombre) in modulos)
        {
            var permisoModulo = new PermisosModulo
            {
                NombreModulo = key,
                Ver = false,
                Crear = false,
                Editar = false,
                Eliminar = false
            };
            
            viewModel.Permisos[key] = permisoModulo;
            viewModel.PermisosModulo[key] = permisoModulo;
        }

        return viewModel;
    }
}
