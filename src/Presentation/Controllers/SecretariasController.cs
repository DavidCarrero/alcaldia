using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Common;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class SecretariasController : BaseController
{
    private readonly ISecretariaService _secretariaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILogger<SecretariasController> _logger;

    public SecretariasController(
        ISecretariaService secretariaService,
        IAlcaldiaService alcaldiaService,
        ILogger<SecretariasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _secretariaService = secretariaService;
        _alcaldiaService = alcaldiaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Secretarias
            .Include(s => s.SecretariasSubsecretarias)
            .Include(s => s.Alcaldia)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(s => 
                s.Codigo.ToLower().Contains(searchLower) || 
                s.Nombre.ToLower().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(s => s.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SecretariaViewModel
            {
                Id = s.Id,
                Codigo = s.Codigo,
                Nombre = s.Nombre,
                Descripcion = s.Descripcion,
                Activo = s.Activo,
                AlcaldiaId = s.AlcaldiaId,
                NitAlcaldia = s.Alcaldia != null ? s.Alcaldia.Nit : ""
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalSecretarias = await _context.Secretarias.CountAsync(s => !s.IsDeleted);
        ViewBag.SecretariasActivas = await _context.Secretarias.CountAsync(s => !s.IsDeleted && s.Activo);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
        ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
        
        var model = new SecretariaViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SecretariaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                TempData["Error"] = "No tiene una alcaldía asignada.";
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _secretariaService.CreateSecretariaAsync(model);
            TempData["Success"] = "Secretaría creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear secretaría");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var secretaria = await _secretariaService.GetSecretariaByIdAsync(id);
        if (secretaria == null)
        {
            return NotFound();
        }

        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
        ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
        return View("Form", secretaria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SecretariaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }

        try
        {
            // Asegurar que el AlcaldiaId se preserve
            if (model.AlcaldiaId == 0)
            {
                model.AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0;
            }
            
            await _secretariaService.UpdateSecretariaAsync(id, model);
            TempData["Success"] = "Secretaría actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar secretaría");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var secretaria = await _secretariaService.GetSecretariaByIdAsync(id);
            if (secretaria == null)
            {
                return Json(new { success = false, message = "La secretaría no fue encontrada." });
            }

            var deletedBy = await ObtenerUsuarioIdActual();
            await _secretariaService.DeleteSecretariaAsync(id, deletedBy);
            return Json(new { success = true, message = $"La secretaría '{secretaria.Nombre}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar secretaría");
            return Json(new { success = false, message = "Error al eliminar la secretaría." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAlcaldias(string searchTerm = "")
    {
        // Solo devolver alcaldías activas para dropdowns
        var alcaldias = string.IsNullOrEmpty(searchTerm)
            ? await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false)
            : await _alcaldiaService.SearchAlcaldiasAsync(searchTerm);

        return Json(alcaldias.Select(a => new {
            id = a.Id,
            nit = a.Nit,
            municipio = a.NombreMunicipio
        }));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            // Solo devolver activas para dropdowns
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                secretarias = secretarias.Where(s => 
                    s.Nombre.ToLower().Contains(term));
            }

            return Json(secretarias
                .Take(limit)
                .Select(s => new {
                    id = s.Id,
                    nombre = s.Nombre
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar secretarías");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            // Obtener el máximo ID excluyendo los eliminados con soft delete
            var maxId = await _context.Secretarias
                .Where(s => !s.IsDeleted)
                .Select(s => s.Id)
                .MaxAsync();
            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener siguiente ID");
            // Si no hay registros, MaxAsync lanza excepción, retornar 1
            return Json(new { nextId = 1 });
        }
    }
}
