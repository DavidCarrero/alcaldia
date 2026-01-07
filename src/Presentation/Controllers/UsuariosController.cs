using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Application.DTOs;
using Proyecto_alcaldia.Infrastructure.Data;
using System.Security.Claims;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;
    private readonly IRolService _rolService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioService usuarioService,
        IRolService rolService,
        ILogger<UsuariosController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _usuarioService = usuarioService;
        _rolService = rolService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Usuarios
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(u => 
                u.NombreCompleto.ToLower().Contains(searchLower) || 
                u.CorreoElectronico.ToLower().Contains(searchLower) ||
                (u.NombreUsuario != null && u.NombreUsuario.ToLower().Contains(searchLower)));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderByDescending(u => u.Id);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                NombreUsuario = u.NombreUsuario,
                CorreoElectronico = u.CorreoElectronico,
                Activo = u.Activo,
                UltimoAcceso = u.UltimoAcceso
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas (del total, no de la página)
        var estadisticas = await _usuarioService.GetEstadisticasAsync();
        ViewBag.UsuariosActivos = estadisticas.activos;
        ViewBag.UsuariosInactivos = estadisticas.inactivos;
        ViewBag.SesionesActivas = estadisticas.sesionesActivas;
        ViewBag.AccesosHoy = estadisticas.accesosHoy;

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var roles = await _rolService.GetAllRolesAsync();
        ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
        var model = new CrearUsuarioViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var roles = await _rolService.GetAllRolesAsync();
            ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var roles = await _rolService.GetAllRolesAsync();
                ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _usuarioService.CreateUsuarioAsync(model);
            TempData["Success"] = "Usuario creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            var roles = await _rolService.GetAllRolesAsync();
            ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var model = new EditarUsuarioViewModel
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            CorreoElectronico = usuario.CorreoElectronico,
            NombreUsuario = usuario.NombreUsuario,
            Activo = usuario.Activo,
            RolId = usuario.RolId,
            UltimoAcceso = usuario.UltimoAcceso
        };

        var roles = await _rolService.GetAllRolesAsync();
        ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
        return View("Form", model);
    }

    [HttpPatch]
    [HttpPost] // Permite POST también por compatibilidad de formularios
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditarUsuarioViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var roles = await _rolService.GetAllRolesAsync();
            ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
            return View("Form", model);
        }

        try
        {
            await _usuarioService.UpdateUsuarioAsync(id, model);
            TempData["Success"] = "Usuario actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            var roles = await _rolService.GetAllRolesAsync();
            ViewBag.Roles = roles.Select(r => new { Id = r.Id, Text = r.Nombre });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "El usuario no fue encontrado." });
            }

            await _usuarioService.DeleteUsuarioAsync(id);
            return Json(new { success = true, message = $"El usuario '{usuario.NombreCompleto}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar usuario");
            return Json(new { success = false, message = "Error al eliminar el usuario." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var usuarios = await _usuarioService.SearchUsuariosAsync(term);
        return Json(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email, int? id)
    {
        var exists = await _usuarioService.EmailExistsAsync(email, id);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> CheckUsername(string username, int? id)
    {
        var exists = await _usuarioService.UsernameExistsAsync(username, id);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles(string search = "")
    {
        // Solo devolver roles activos para dropdowns
        var roles = string.IsNullOrEmpty(search) 
            ? await _rolService.GetAllRolesAsync(incluirInactivos: false)
            : await _rolService.SearchRolesAsync(search);
        var result = roles.Select(r => new { id = r.Id, nombre = r.Nombre });
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        var nextId = await GetNextCodigoAsync("Usuarios");
        return Json(new { nextId = nextId });
    }

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var usuario = await _usuarioService.GetUsuarioByEmailAsync(userEmail);
        if (usuario == null)
        {
            return NotFound();
        }

        return View("Perfil", usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarPerfil(UsuarioDto model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var usuario = await _usuarioService.GetUsuarioByEmailAsync(userEmail);
            if (usuario == null)
            {
                return NotFound();
            }

            // Actualizar el perfil usando el nuevo método
            await _usuarioService.ActualizarPerfilAsync(usuario.Id, model);
            TempData["Success"] = "Perfil actualizado exitosamente";
            return RedirectToAction(nameof(Perfil));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil");
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Perfil));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(int id, string passwordActual, string nuevaPassword, string confirmarPassword)
    {
        if (string.IsNullOrEmpty(passwordActual) || string.IsNullOrEmpty(nuevaPassword) || string.IsNullOrEmpty(confirmarPassword))
        {
            TempData["Error"] = "Todos los campos son requeridos";
            return RedirectToAction(nameof(Perfil));
        }

        if (nuevaPassword != confirmarPassword)
        {
            TempData["Error"] = "La nueva contraseña y la confirmación no coinciden";
            return RedirectToAction(nameof(Perfil));
        }

        try
        {
            await _usuarioService.CambiarPasswordAsync(id, passwordActual, nuevaPassword);
            TempData["Success"] = "Contraseña cambiada exitosamente";
            return RedirectToAction(nameof(Perfil));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña");
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Perfil));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarTemaColor(string temaColor)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail))
        {
            return Json(new { success = false, message = "Usuario no autenticado" });
        }

        try
        {
            var usuario = await _usuarioService.GetUsuarioByEmailAsync(userEmail);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            await _usuarioService.CambiarTemaColorAsync(usuario.Id, temaColor);
            return Json(new { success = true, message = "Tema actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar tema");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarFotoPerfil(IFormFile foto)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail))
        {
            return Json(new { success = false, message = "Usuario no autenticado" });
        }

        try
        {
            if (foto == null || foto.Length == 0)
            {
                return Json(new { success = false, message = "No se seleccionó ninguna foto" });
            }

            // Validar tamaño (máximo 2MB)
            if (foto.Length > 2 * 1024 * 1024)
            {
                return Json(new { success = false, message = "La imagen no debe superar 2MB" });
            }

            // Validar tipo de archivo
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Solo se permiten imágenes JPG y PNG" });
            }

            var usuario = await _usuarioService.GetUsuarioByEmailAsync(userEmail);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // Leer los bytes de la foto
            using var memoryStream = new MemoryStream();
            await foto.CopyToAsync(memoryStream);
            var fotoBytes = memoryStream.ToArray();

            // Actualizar la foto (será encriptada automáticamente en el servicio)
            await _usuarioService.ActualizarFotoPerfilAsync(usuario.Id, fotoBytes);

            return Json(new { success = true, message = "Foto de perfil actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar foto de perfil");
            return Json(new { success = false, message = "Error al actualizar la foto de perfil" });
        }
    }
}

