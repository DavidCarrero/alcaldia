using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class SectoresController : BaseController
{
    private readonly ISectorService _sectorService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILineaEstrategicaService _lineaEstrategicaService;
    private readonly ILogger<SectoresController> _logger;

    public SectoresController(
        ISectorService sectorService,
        IAlcaldiaService alcaldiaService,
        ILineaEstrategicaService lineaEstrategicaService,
        ILogger<SectoresController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _sectorService = sectorService;
        _alcaldiaService = alcaldiaService;
        _lineaEstrategicaService = lineaEstrategicaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: true);
        return View(sectores);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
        ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });

        var model = new SectorViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SectorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
                ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            model.LineaEstrategicaId = NormalizarIdOpcional(model.LineaEstrategicaId);
            
            await _sectorService.CreateSectorAsync(model);
            TempData["Success"] = "Sector creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sector");
            ModelState.AddModelError("", ex.Message);
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sector = await _sectorService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }

        var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
        ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });

        return View("Form", sector);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SectorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _sectorService.UpdateSectorAsync(id, model);
            TempData["Success"] = "Sector actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sector");
            ModelState.AddModelError("", ex.Message);
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sector = await _sectorService.GetSectorByIdAsync(id);
            if (sector == null)
            {
                return Json(new { success = false, message = "El sector no fue encontrado." });
            }

            await _sectorService.DeleteSectorAsync(id);
            return Json(new { success = true, message = $"El sector '{sector.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar sector");
            return Json(new { success = false, message = "Error al eliminar el sector." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                sectores = sectores.Where(s => 
                    s.Codigo.ToLower().Contains(term) || 
                    s.Nombre.ToLower().Contains(term));
            }

            var result = sectores
                .Take(limit)
                .Select(s => new {
                    id = s.Id,
                    codigo = s.Codigo,
                    nombre = s.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar sectores");
            return Json(new List<object>());
        }
    }
}
