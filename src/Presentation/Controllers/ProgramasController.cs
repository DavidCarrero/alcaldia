using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class ProgramasController : BaseController
{
    private readonly IProgramaService _programaService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IPlanMunicipalService _planMunicipalService;
    private readonly ISectorService _sectorService;
    private readonly IODSService _odsService;
    private readonly ILogger<ProgramasController> _logger;

    public ProgramasController(
        IProgramaService programaService,
        IAlcaldiaService alcaldiaService,
        IPlanMunicipalService planMunicipalService,
        ISectorService sectorService,
        IODSService odsService,
        ILogger<ProgramasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _programaService = programaService;
        _alcaldiaService = alcaldiaService;
        _planMunicipalService = planMunicipalService;
        _sectorService = sectorService;
        _odsService = odsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: true);
        return View(programas);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
        ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
        ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

        return View("Form", new ProgramaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProgramaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
            ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
            ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

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

            await _programaService.CreateProgramaAsync(model);
            TempData["Success"] = "Programa creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear programa");
            ModelState.AddModelError("", ex.Message);

            var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
            ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
            ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var programa = await _programaService.GetProgramaByIdAsync(id);
        if (programa == null)
        {
            return NotFound();
        }

        var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
        ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
        ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

        return View("Form", programa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProgramaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
            ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
            ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

            return View("Form", model);
        }

        try
        {
            await _programaService.UpdateProgramaAsync(id, model);
            TempData["Success"] = "Programa actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar programa");
            ModelState.AddModelError("", ex.Message);

            var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
            ViewBag.PlanesMunicipales = planesMunicipales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            var ods = await _odsService.GetAllODSAsync(incluirInactivos: false);
            ViewBag.ODS = ods.Select(o => new { Id = o.Id, Text = $"{o.Codigo} - {o.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var programa = await _programaService.GetProgramaByIdAsync(id);
            if (programa == null)
            {
                return Json(new { success = false, message = "El programa no fue encontrado." });
            }

            await _programaService.DeleteProgramaAsync(id);
            return Json(new { success = true, message = $"El programa '{programa.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar programa");
            return Json(new { success = false, message = "Error al eliminar el programa." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                programas = programas.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = programas
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
            _logger.LogError(ex, "Error al buscar programas");
            return Json(new List<object>());
        }
    }
}
