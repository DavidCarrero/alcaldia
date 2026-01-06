using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class EvidenciasController : BaseController
{
    private readonly IEvidenciaService _evidenciaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IActividadService _actividadService;
    private readonly ILogger<EvidenciasController> _logger;

    public EvidenciasController(
        IEvidenciaService evidenciaService,
        IAlcaldiaService alcaldiaService,
        IActividadService actividadService,
        ILogger<EvidenciasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _evidenciaService = evidenciaService;
        _alcaldiaService = alcaldiaService;
        _actividadService = actividadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Evidencias
            .Include(e => e.Actividad)
            .Include(e => e.Alcaldia)
            .Where(e => !e.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(e => 
                e.Codigo.ToLower().Contains(searchLower) || 
                e.Nombre.ToLower().Contains(searchLower) ||
                (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchLower)));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(e => e.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EvidenciaViewModel
            {
                Id = e.Id,
                Codigo = e.Codigo,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                ActividadId = e.ActividadId,
                NombreActividad = e.Actividad != null ? e.Actividad.Nombre : "",
                AlcaldiaId = e.AlcaldiaId
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalEvidencias = totalCount;
        ViewBag.TotalConActividad = await _context.Evidencias.Where(e => !e.IsDeleted && e.ActividadId != null).CountAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
        ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });

        var model = new EvidenciaViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EvidenciaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
            ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
                ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            model.ActividadId = NormalizarIdOpcional(model.ActividadId);
            
            await _evidenciaService.CreateEvidenciaAsync(model);
            TempData["Success"] = "Evidencia creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear evidencia");
            ModelState.AddModelError("", ex.Message);
            var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
            ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var evidencia = await _evidenciaService.GetEvidenciaByIdAsync(id);
        if (evidencia == null)
        {
            return NotFound();
        }

        var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
        ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });

        return View("Form", evidencia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EvidenciaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
            ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _evidenciaService.UpdateEvidenciaAsync(id, model);
            TempData["Success"] = "Evidencia actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar evidencia");
            ModelState.AddModelError("", ex.Message);
            var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
            ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var evidencia = await _evidenciaService.GetEvidenciaByIdAsync(id);
            if (evidencia == null)
            {
                return Json(new { success = false, message = "La evidencia no fue encontrada." });
            }

            await _evidenciaService.DeleteEvidenciaAsync(id);
            return Json(new { success = true, message = $"La evidencia '{evidencia.Codigo}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar evidencia");
            return Json(new { success = false, message = "Error al eliminar la evidencia." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var evidencias = await _evidenciaService.GetAllEvidenciasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                evidencias = evidencias.Where(e => 
                    e.Codigo.ToLower().Contains(term) || 
                    e.Nombre.ToLower().Contains(term));
            }

            var result = evidencias
                .Take(limit)
                .Select(e => new {
                    id = e.Id,
                    codigo = e.Codigo,
                    nombre = e.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar evidencias");
            return Json(new List<object>());
        }
    }

    // GET: Evidencias/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Evidencias
                .Where(e => !e.IsDeleted)
                .Select(e => e.Id)
                .DefaultIfEmpty(0)
                .MaxAsync();

            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception)
        {
            return Json(new { nextId = 1 });
        }
    }
}
