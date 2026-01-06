using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class MetasODSController : BaseController
{
    private readonly IMetaODSService _metasODSService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILogger<MetasODSController> _logger;

    public MetasODSController(
        IMetaODSService metasODSService,
        IAlcaldiaService alcaldiaService,
        ILogger<MetasODSController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _metasODSService = metasODSService;
        _alcaldiaService = alcaldiaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var metasODS = await _metasODSService.GetAllMetasODSAsync(incluirInactivas: true);
        return View(metasODS);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
        ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
        
        var model = new MetaODSViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MetaODSViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
                ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _metasODSService.CreateMetaODSAsync(model);
            TempData["Success"] = "Meta ODS creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear Meta ODS");
            ModelState.AddModelError("", ex.Message);
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var metaODS = await _metasODSService.GetMetaODSByIdAsync(id);
        if (metaODS == null)
        {
            return NotFound();
        }

        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
        ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
        return View("Form", metaODS);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MetaODSViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }

        try
        {
            await _metasODSService.UpdateMetaODSAsync(id, model);
            TempData["Success"] = "Meta ODS actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar Meta ODS");
            ModelState.AddModelError("", ex.Message);
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            ViewBag.Alcaldias = alcaldias.Select(a => new { Id = a.Id, Text = $"{a.Nit} - {a.NombreMunicipio}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var metaODS = await _metasODSService.GetMetaODSByIdAsync(id);
            if (metaODS == null)
            {
                return Json(new { success = false, message = "La Meta ODS no fue encontrada." });
            }

            await _metasODSService.DeleteMetaODSAsync(id);
            return Json(new { success = true, message = $"La Meta ODS '{metaODS.Nombre}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar Meta ODS");
            return Json(new { success = false, message = "Error al eliminar la Meta ODS." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAlcaldias(string searchTerm = "")
    {
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
            var metasODS = await _metasODSService.GetAllMetasODSAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                metasODS = metasODS.Where(m => 
                    m.Codigo.ToLower().Contains(term) ||
                    m.Nombre.ToLower().Contains(term));
            }

            return Json(metasODS
                .Take(limit)
                .Select(m => new {
                    id = m.Id,
                    codigo = m.Codigo,
                    nombre = m.Nombre
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar Metas ODS");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckCodigo(string codigo, int alcaldiaId, int? id)
    {
        var exists = await _metasODSService.CodigoExistsAsync(codigo, alcaldiaId, id);
        return Json(!exists);
    }
}