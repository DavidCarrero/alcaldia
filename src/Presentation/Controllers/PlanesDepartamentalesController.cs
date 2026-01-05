using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class PlanesDepartamentalesController : BaseController
{
    private readonly IPlanDepartamentalService _planDepartamentalService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ISectorService _sectorService;
    private readonly ILogger<PlanesDepartamentalesController> _logger;

    public PlanesDepartamentalesController(
        IPlanDepartamentalService planDepartamentalService,
        IAlcaldiaService alcaldiaService,
        ISectorService sectorService,
        ILogger<PlanesDepartamentalesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _planDepartamentalService = planDepartamentalService;
        _alcaldiaService = alcaldiaService;
        _sectorService = sectorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: true);
        return View(planesDepartamentales);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        return View("Form", new PlanDepartamentalViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanDepartamentalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Asignar alcaldía automáticamente desde la primera alcaldía activa
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            var primeraAlcaldia = alcaldias.FirstOrDefault();
            if (primeraAlcaldia != null)
            {
                model.AlcaldiaId = primeraAlcaldia.Id;
            }

            await _planDepartamentalService.CreatePlanDepartamentalAsync(model);
            TempData["Success"] = "Plan departamental creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plan departamental");
            ModelState.AddModelError("", ex.Message);
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var planDepartamental = await _planDepartamentalService.GetPlanDepartamentalByIdAsync(id);
        if (planDepartamental == null)
        {
            return NotFound();
        }

        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        return View("Form", planDepartamental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanDepartamentalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _planDepartamentalService.UpdatePlanDepartamentalAsync(id, model);
            TempData["Success"] = "Plan departamental actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plan departamental");
            ModelState.AddModelError("", ex.Message);
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var planDepartamental = await _planDepartamentalService.GetPlanDepartamentalByIdAsync(id);
            if (planDepartamental == null)
            {
                return Json(new { success = false, message = "El plan departamental no fue encontrado." });
            }

            await _planDepartamentalService.DeletePlanDepartamentalAsync(id);
            return Json(new { success = true, message = $"El plan departamental '{planDepartamental.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar plan departamental");
            return Json(new { success = false, message = "Error al eliminar el plan departamental." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                planesDepartamentales = planesDepartamentales.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = planesDepartamentales
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
            _logger.LogError(ex, "Error al buscar planes departamentales");
            return Json(new List<object>());
        }
    }
}
