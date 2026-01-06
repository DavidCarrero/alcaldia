using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class VigenciasController : BaseController
{
    private readonly IVigenciaService _vigenciaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILogger<VigenciasController> _logger;

    public VigenciasController(
        IVigenciaService vigenciaService,
        IAlcaldiaService alcaldiaService,
        ILogger<VigenciasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _vigenciaService = vigenciaService;
        _alcaldiaService = alcaldiaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Vigencias
            .Where(v => !v.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(v => 
                v.Codigo.ToLower().Contains(searchLower) || 
                v.Nombre.ToLower().Contains(searchLower) ||
                v.Año.ToString().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderByDescending(v => v.Año);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VigenciaViewModel
            {
                Id = v.Id,
                Codigo = v.Codigo,
                Nombre = v.Nombre,
                Año = v.Año,
                FechaInicio = v.FechaInicio,
                FechaFin = v.FechaFin,
                Activo = v.Activo
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalVigencias = await _context.Vigencias.CountAsync(v => !v.IsDeleted);
        ViewBag.VigenciasActivas = await _context.Vigencias.CountAsync(v => !v.IsDeleted && v.Activo);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new VigenciaViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VigenciaViewModel model)
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
            
            await _vigenciaService.CreateVigenciaAsync(model);
            TempData["Success"] = "Vigencia creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vigencia");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var vigencia = await _vigenciaService.GetVigenciaByIdAsync(id);
        if (vigencia == null)
        {
            return NotFound();
        }

        return View("Form", vigencia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VigenciaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            await _vigenciaService.UpdateVigenciaAsync(id, model);
            TempData["Success"] = "Vigencia actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar vigencia");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var vigencia = await _vigenciaService.GetVigenciaByIdAsync(id);
            if (vigencia == null)
            {
                return Json(new { success = false, message = "La vigencia no fue encontrada." });
            }

            await _vigenciaService.DeleteVigenciaAsync(id);
            return Json(new { success = true, message = $"La vigencia '{vigencia.Codigo}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar vigencia");
            return Json(new { success = false, message = "Error al eliminar la vigencia." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                vigencias = vigencias.Where(v => 
                    (v.Codigo != null && v.Codigo.ToLower().Contains(term)) || 
                    v.Nombre.ToLower().Contains(term));
            }

            var result = vigencias
                .Take(limit)
                .Select(v => new {
                    id = v.Id,
                    codigo = v.Codigo,
                    nombre = v.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar vigencias");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNextCodigo()
    {
        try
        {
            var maxId = await _context.Vigencias
                .MaxAsync(v => (int?)v.Id) ?? 0;
            
            return Json(new { nextCodigo = (maxId + 1).ToString() });
        }
        catch (Exception)
        {
            return Json(new { nextCodigo = "1" });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Vigencias
                .MaxAsync(v => (int?)v.Id) ?? 0;
            
            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception)
        {
            return Json(new { nextId = 1 });
        }
    }
}
