using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Application.DTOs;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Domain.Interfaces;

namespace Proyecto_alcaldia.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IImageEncryptionService _imageEncryptionService;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IImageEncryptionService imageEncryptionService,
        ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _imageEncryptionService = imageEncryptionService;
        _logger = logger;
    }

    public async Task<IEnumerable<UsuarioViewModel>> GetAllUsuariosAsync(bool incluirInactivos = false)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(incluirInactivos);
        return usuarios.Select(MapToViewModel);
    }

    public async Task<UsuarioViewModel?> GetUsuarioByIdAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        return usuario != null ? MapToViewModel(usuario) : null;
    }

    public async Task<UsuarioViewModel> CreateUsuarioAsync(CrearUsuarioViewModel model)
    {
        // Validar email único
        if (await _usuarioRepository.EmailExistsAsync(model.CorreoElectronico))
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado");
        }

        // Validar username único si se proporciona
        if (!string.IsNullOrEmpty(model.NombreUsuario) && 
            await _usuarioRepository.UsernameExistsAsync(model.NombreUsuario))
        {
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        var usuario = new Usuario
        {
            NombreCompleto = model.NombreCompleto,
            CorreoElectronico = model.CorreoElectronico,
            NombreUsuario = model.NombreUsuario,
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.Contrasena),
            Activo = model.Activo,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow,
            UsuariosRoles = new List<UsuarioRol>()
        };

        // Asignar roles
        if (model.RolesIds != null && model.RolesIds.Any())
        {
            foreach (var rolId in model.RolesIds)
            {
                usuario.UsuariosRoles.Add(new UsuarioRol
                {
                    RolId = rolId,
                    FechaAsignacion = DateTime.UtcNow,
                    Activo = true
                });
            }
        }

        await _usuarioRepository.CreateAsync(usuario);
        _logger.LogInformation($"Usuario creado: {usuario.CorreoElectronico}");

        return MapToViewModel(usuario);
    }

    public async Task UpdateUsuarioAsync(int id, EditarUsuarioViewModel model)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        // Validar email único
        if (await _usuarioRepository.EmailExistsAsync(model.CorreoElectronico, id))
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado");
        }

        // Validar username único
        if (!string.IsNullOrEmpty(model.NombreUsuario) &&
            await _usuarioRepository.UsernameExistsAsync(model.NombreUsuario, id))
        {
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        usuario.NombreCompleto = model.NombreCompleto;
        usuario.CorreoElectronico = model.CorreoElectronico;
        usuario.NombreUsuario = model.NombreUsuario;
        usuario.Activo = model.Activo;

        // Actualizar contraseña si se proporciona
        if (!string.IsNullOrEmpty(model.NuevaContrasena))
        {
            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);
        }

        // Actualizar roles - eliminar todos los anteriores y agregar los nuevos
        usuario.UsuariosRoles.Clear();
        if (model.RolesIds != null && model.RolesIds.Any())
        {
            foreach (var rolId in model.RolesIds)
            {
                usuario.UsuariosRoles.Add(new UsuarioRol
                {
                    UsuarioId = id,
                    RolId = rolId,
                    FechaAsignacion = DateTime.UtcNow,
                    Activo = true
                });
            }
        }

        await _usuarioRepository.UpdateAsync(usuario);
        _logger.LogInformation($"Usuario actualizado: {usuario.CorreoElectronico}");
    }

    public async Task DeleteUsuarioAsync(int id)
    {
        await _usuarioRepository.DeleteAsync(id);
        _logger.LogInformation($"Usuario desactivado: ID {id}");
    }

    public async Task<IEnumerable<UsuarioViewModel>> SearchUsuariosAsync(string searchTerm)
    {
        var usuarios = await _usuarioRepository.SearchAsync(searchTerm);
        return usuarios.Select(MapToViewModel);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await _usuarioRepository.EmailExistsAsync(email, excludeUserId);
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
    {
        return await _usuarioRepository.UsernameExistsAsync(username, excludeUserId);
    }

    public async Task<(int activos, int inactivos, int sesionesActivas, int accesosHoy)> GetEstadisticasAsync()
    {
        var activos = await _usuarioRepository.CountActiveUsersAsync();
        var inactivos = await _usuarioRepository.CountInactiveUsersAsync();
        
        // Por ahora valores simulados - se pueden implementar con tablas de sesiones
        var sesionesActivas = activos > 0 ? activos / 2 : 0;
        var accesosHoy = activos > 0 ? activos - 5 : 0;

        return (activos, inactivos, sesionesActivas, accesosHoy);
    }

    public async Task<UsuarioDto?> GetUsuarioByEmailAsync(string email)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        return usuario != null ? MapToDto(usuario) : null;
    }

    public async Task<bool> CambiarPasswordAsync(int usuarioId, string passwordActual, string nuevaPassword)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
            return false;

        // Verificar password actual
        if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.ContrasenaHash))
            return false;

        // Actualizar con nueva password
        usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
        await _usuarioRepository.UpdateAsync(usuario);
        
        return true;
    }

    public async Task CambiarTemaColorAsync(int usuarioId, string temaColor)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.TemaColor = temaColor;
        await _usuarioRepository.UpdateAsync(usuario);
    }

    public async Task ActualizarPerfilAsync(int usuarioId, UsuarioDto dto)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        // Validar email único
        if (usuario.CorreoElectronico != dto.CorreoElectronico && 
            await _usuarioRepository.EmailExistsAsync(dto.CorreoElectronico, usuarioId))
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado");
        }

        // Validar username único
        if (usuario.NombreUsuario != dto.NombreUsuario && 
            !string.IsNullOrEmpty(dto.NombreUsuario) &&
            await _usuarioRepository.UsernameExistsAsync(dto.NombreUsuario, usuarioId))
        {
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.CorreoElectronico = dto.CorreoElectronico;
        usuario.NombreUsuario = dto.NombreUsuario;

        await _usuarioRepository.UpdateAsync(usuario);
    }

    public async Task ActualizarFotoPerfilAsync(int usuarioId, byte[] fotoBytes)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        // Encriptar la foto antes de guardarla
        usuario.FotoPerfilEncriptada = _imageEncryptionService.EncryptImage(fotoBytes);
        
        await _usuarioRepository.UpdateAsync(usuario);
        _logger.LogInformation($"Foto de perfil actualizada para usuario ID: {usuarioId}");
    }

    private UsuarioDto MapToDto(Usuario usuario)
    {
        string? fotoBase64 = null;
        
        // Desencriptar la foto si existe
        if (!string.IsNullOrEmpty(usuario.FotoPerfilEncriptada))
        {
            try
            {
                var fotoBytes = _imageEncryptionService.DecryptImage(usuario.FotoPerfilEncriptada);
                fotoBase64 = Convert.ToBase64String(fotoBytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error al desencriptar foto de perfil para usuario ID: {usuario.Id}");
            }
        }

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            NombreUsuario = usuario.NombreUsuario,
            Activo = usuario.Activo,
            UltimoAcceso = usuario.UltimoAcceso,
            FechaCreacion = usuario.FechaCreacion,
            TemaColor = usuario.TemaColor,
            FotoPerfil = fotoBase64,
            Roles = usuario.UsuariosRoles
                .Where(ur => ur.Activo)
                .Select(ur => ur.Rol.Nombre)
                .ToList()
        };
    }

    private static UsuarioViewModel MapToViewModel(Usuario usuario)
    {
        return new UsuarioViewModel
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            NombreUsuario = usuario.NombreUsuario,
            Activo = usuario.Activo,
            UltimoAcceso = usuario.UltimoAcceso,
            RolesSeleccionados = usuario.UsuariosRoles
                .Where(ur => ur.Activo)
                .Select(ur => ur.RolId)
                .ToList(),
            RolesIds = usuario.UsuariosRoles
                .Where(ur => ur.Activo)
                .Select(ur => ur.RolId)
                .ToList()
        };
    }
}
