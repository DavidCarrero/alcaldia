using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class SecretariasController : Controller
{
    private readonly ISecretariaService _secretariaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILogger<SecretariasController> _logger;

    public SecretariasController(
        ISecretariaService secretariaService,
        IAlcaldiaService alcaldiaService,
        ILogger<SecretariasController> logger)
    {
        _secretariaService = secretariaService;
        _alcaldiaService = alcaldiaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: true);
        return View(secretarias);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
        ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
        return View("Form", new SecretariaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SecretariaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _secretariaService.CreateSecretariaAsync(model);
            TempData["Success"] = "Secretaría creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear secretaría");
            ModelState.AddModelError("", ex.Message);
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
            return View("Form", model);
        }

        try
        {
            await _secretariaService.UpdateSecretariaAsync(id, model);
            TempData["Success"] = "Secretaría actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar secretaría");
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
            var secretaria = await _secretariaService.GetSecretariaByIdAsync(id);
            if (secretaria == null)
            {
                return Json(new { success = false, message = "La secretaría no fue encontrada." });
            }

            await _secretariaService.DeleteSecretariaAsync(id);
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
}
