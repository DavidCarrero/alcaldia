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
    private readonly ILogger<LineasEstrategicasController> _logger;

    public LineasEstrategicasController(
        ILineaEstrategicaService lineaEstrategicaService,
        IAlcaldiaService alcaldiaService,
        ILogger<LineasEstrategicasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _lineaEstrategicaService = lineaEstrategicaService;
        _alcaldiaService = alcaldiaService;
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
        return View("Form", new LineaEstrategicaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LineaEstrategicaViewModel model)
    {
        if (!ModelState.IsValid)
        {
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

            await _lineaEstrategicaService.CreateLineaEstrategicaAsync(model);
            TempData["Success"] = "Línea estratégica creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear línea estratégica");
            ModelState.AddModelError("", ex.Message);
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

        return View("Form", lineaEstrategica);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LineaEstrategicaViewModel model)
    {
        if (!ModelState.IsValid)
        {
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
