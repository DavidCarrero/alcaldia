using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class LineasEstrategicasController : BaseController
{
    private readonly ILineaEstrategicaService _lineaEstrategicaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IPlanDepartamentalService _planDepartamentalService;
    private readonly ILogger<LineasEstrategicasController> _logger;

    public LineasEstrategicasController(
        ILineaEstrategicaService lineaEstrategicaService,
        IAlcaldiaService alcaldiaService,
        IPlanDepartamentalService planDepartamentalService,
        ILogger<LineasEstrategicasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _lineaEstrategicaService = lineaEstrategicaService;
        _alcaldiaService = alcaldiaService;
        _planDepartamentalService = planDepartamentalService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: true);
        return View(lineasEstrategicas);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
        ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
        
        var model = new LineaEstrategicaViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LineaEstrategicaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
                ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _lineaEstrategicaService.CreateLineaEstrategicaAsync(model);
            TempData["Success"] = "Línea estratégica creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear línea estratégica");
            ModelState.AddModelError("", ex.Message);
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var lineaEstrategica = await _lineaEstrategicaService.GetLineaEstrategicaByIdAsync(id);
        if (lineaEstrategica == null)
        {
            return NotFound();
        }

        var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
        ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
        
        return View("Form", lineaEstrategica);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LineaEstrategicaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _lineaEstrategicaService.UpdateLineaEstrategicaAsync(id, model);
            TempData["Success"] = "Línea estratégica actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar línea estratégica");
            ModelState.AddModelError("", ex.Message);
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var lineaEstrategica = await _lineaEstrategicaService.GetLineaEstrategicaByIdAsync(id);
            if (lineaEstrategica == null)
            {
                return Json(new { success = false, message = "La línea estratégica no fue encontrada." });
            }

            await _lineaEstrategicaService.DeleteLineaEstrategicaAsync(id);
            return Json(new { success = true, message = $"La línea estratégica '{lineaEstrategica.Codigo}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar línea estratégica");
            return Json(new { success = false, message = "Error al eliminar la línea estratégica." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                lineasEstrategicas = lineasEstrategicas.Where(l => 
                    l.Codigo.ToLower().Contains(term) || 
                    l.Nombre.ToLower().Contains(term));
            }

            var result = lineasEstrategicas
                .Take(limit)
                .Select(l => new {
                    id = l.Id,
                    codigo = l.Codigo,
                    nombre = l.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar líneas estratégicas");
            return Json(new List<object>());
        }
    }
}
