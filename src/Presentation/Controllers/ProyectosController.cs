using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class ProyectosController : BaseController
{
    private readonly IProyectoService _proyectoService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IResponsableService _responsableService;
    private readonly ILogger<ProyectosController> _logger;

    public ProyectosController(
        IProyectoService proyectoService,
        IAlcaldiaService alcaldiaService,
        IResponsableService responsableService,
        ILogger<ProyectosController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _proyectoService = proyectoService;
        _alcaldiaService = alcaldiaService;
        _responsableService = responsableService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: true);
        return View(proyectos);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        var model = new ProyectoViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProyectoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
                ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _proyectoService.CreateProyectoAsync(model);
            TempData["Success"] = "Proyecto creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proyecto");
            ModelState.AddModelError("", ex.Message);
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var proyecto = await _proyectoService.GetProyectoByIdAsync(id);
        if (proyecto == null)
        {
            return NotFound();
        }

        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        return View("Form", proyecto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProyectoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });
            return View("Form", model);
        }

        try
        {
            await _proyectoService.UpdateProyectoAsync(id, model);
            TempData["Success"] = "Proyecto actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proyecto");
            ModelState.AddModelError("", ex.Message);
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var proyecto = await _proyectoService.GetProyectoByIdAsync(id);
            if (proyecto == null)
            {
                return Json(new { success = false, message = "El proyecto no fue encontrado." });
            }

            await _proyectoService.DeleteProyectoAsync(id);
            return Json(new { success = true, message = $"El proyecto '{proyecto.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar proyecto");
            return Json(new { success = false, message = "Error al eliminar el proyecto." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var proyectos = await _proyectoService.GetAllProyectosAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                proyectos = proyectos.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = proyectos
                .Take(limit)
                .Select(p => new {
                    id = p.Id,
                    codigo = p.Codigo,
                    nombre = p.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar proyectos");
            return Json(new List<object>());
        }
    }
}
