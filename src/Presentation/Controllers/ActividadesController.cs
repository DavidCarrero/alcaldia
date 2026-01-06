using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class ActividadesController : BaseController
{
    private readonly IActividadService _actividadService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IProyectoService _proyectoService;
    private readonly IResponsableService _responsableService;
    private readonly IVigenciaService _vigenciaService;
    private readonly ILogger<ActividadesController> _logger;

    public ActividadesController(
        IActividadService actividadService,
        IAlcaldiaService alcaldiaService,
        IProyectoService proyectoService,
        IResponsableService responsableService,
        IVigenciaService vigenciaService,
        ILogger<ActividadesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _actividadService = actividadService;
        _alcaldiaService = alcaldiaService;
        _proyectoService = proyectoService;
        _responsableService = responsableService;
        _vigenciaService = vigenciaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: true);
        return View(actividades);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
        ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
        ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

        var model = new ActividadViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActividadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
            ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
            ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
                ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

                var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
                ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _actividadService.CreateActividadAsync(model);
            TempData["Success"] = "Actividad creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear actividad");
            ModelState.AddModelError("", ex.Message);

            var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
            ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
            ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var actividad = await _actividadService.GetActividadByIdAsync(id);
        if (actividad == null)
        {
            return NotFound();
        }

        var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
        ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
        ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

        return View("Form", actividad);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActividadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
            ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
            ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

            return View("Form", model);
        }

        try
        {
            await _actividadService.UpdateActividadAsync(id, model);
            TempData["Success"] = "Actividad actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar actividad");
            ModelState.AddModelError("", ex.Message);

            var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
            ViewBag.Proyectos = proyectos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: false);
            ViewBag.Vigencias = vigencias.Select(v => new { Id = v.Id, Text = $"{v.Codigo} - {v.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var actividad = await _actividadService.GetActividadByIdAsync(id);
            if (actividad == null)
            {
                return Json(new { success = false, message = "La actividad no fue encontrada." });
            }

            await _actividadService.DeleteActividadAsync(id);
            return Json(new { success = true, message = $"La actividad '{actividad.Codigo}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar actividad");
            return Json(new { success = false, message = "Error al eliminar la actividad." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                actividades = actividades.Where(a => 
                    a.Codigo.ToLower().Contains(term) || 
                    a.Nombre.ToLower().Contains(term));
            }

            var result = actividades
                .Take(limit)
                .Select(a => new {
                    id = a.Id,
                    codigo = a.Codigo,
                    nombre = a.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar actividades");
            return Json(new List<object>());
        }
    }
}
