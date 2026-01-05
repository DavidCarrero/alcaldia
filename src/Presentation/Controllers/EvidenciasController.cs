using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Index()
    {
        var evidencias = await _evidenciaService.GetAllEvidenciasAsync(incluirInactivas: true);
        return View(evidencias);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var actividades = await _actividadService.GetAllActividadesAsync(incluirInactivas: false);
        ViewBag.Actividades = actividades.Select(a => new { Id = a.Id, Text = $"{a.Codigo} - {a.Nombre}" });

        return View("Form", new EvidenciaViewModel());
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
            // Asignar alcaldía automáticamente desde la primera alcaldía activa
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            var primeraAlcaldia = alcaldias.FirstOrDefault();
            if (primeraAlcaldia != null)
            {
                model.AlcaldiaId = primeraAlcaldia.Id;
            }

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
}
