using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Index()
    {
        var vigencias = await _vigenciaService.GetAllVigenciasAsync(incluirInactivas: true);
        return View(vigencias);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View("Form", new VigenciaViewModel());
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
            // Asignar alcaldía automáticamente desde la primera alcaldía activa
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            var primeraAlcaldia = alcaldias.FirstOrDefault();
            if (primeraAlcaldia != null)
            {
                model.AlcaldiaId = primeraAlcaldia.Id;
            }

            await _vigenciaService.CreateVigenciaAsync(model);
            TempData["Success"] = "Vigencia creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear vigencia");
            ModelState.AddModelError("", ex.Message);
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
}
