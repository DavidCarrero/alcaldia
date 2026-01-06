using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class PlanesMunicipalesController : BaseController
{
    private readonly IPlanMunicipalService _planMunicipalService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IMunicipioService _municipioService;
    private readonly IAlcaldeService _alcaldeService;
    private readonly IPlanNacionalService _planNacionalService;
    private readonly IPlanDepartamentalService _planDepartamentalService;
    private readonly ILogger<PlanesMunicipalesController> _logger;

    public PlanesMunicipalesController(
        IPlanMunicipalService planMunicipalService,
        IAlcaldiaService alcaldiaService,
        IMunicipioService municipioService,
        IAlcaldeService alcaldeService,
        IPlanNacionalService planNacionalService,
        IPlanDepartamentalService planDepartamentalService,
        ILogger<PlanesMunicipalesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _planMunicipalService = planMunicipalService;
        _alcaldiaService = alcaldiaService;
        _municipioService = municipioService;
        _alcaldeService = alcaldeService;
        _planNacionalService = planNacionalService;
        _planDepartamentalService = planDepartamentalService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: true);
        return View(planesMunicipales);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
        ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

        var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
        ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

        var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
        ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
        ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var model = new PlanMunicipalViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanMunicipalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

            var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
            ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

            var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
            ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
                ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

                var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
                ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _planMunicipalService.CreatePlanMunicipalAsync(model);
            TempData["Success"] = "Plan municipal creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plan municipal");
            ModelState.AddModelError("", ex.Message);

            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

            var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
            ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

            var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
            ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var planMunicipal = await _planMunicipalService.GetPlanMunicipalByIdAsync(id);
        if (planMunicipal == null)
        {
            return NotFound();
        }

        var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
        ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

        var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
        ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

        var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
        ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
        ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        return View("Form", planMunicipal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanMunicipalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

            var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
            ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

            var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
            ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            ViewBag.PlanesDepartamentales = planesDepartamentales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }

        try
        {
            await _planMunicipalService.UpdatePlanMunicipalAsync(id, model);
            TempData["Success"] = "Plan municipal actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plan municipal");
            ModelState.AddModelError("", ex.Message);

            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });

            var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: false);
            ViewBag.Alcaldes = alcaldes.Select(a => new { Id = a.Id, Text = $"{a.NumeroDocumento} - {a.NombreCompleto}" });

            var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
            ViewBag.PlanesNacionales = planesNacionales.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

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
            var planMunicipal = await _planMunicipalService.GetPlanMunicipalByIdAsync(id);
            if (planMunicipal == null)
            {
                return Json(new { success = false, message = "El plan municipal no fue encontrado." });
            }

            await _planMunicipalService.DeletePlanMunicipalAsync(id);
            return Json(new { success = true, message = $"El plan municipal '{planMunicipal.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar plan municipal");
            return Json(new { success = false, message = "Error al eliminar el plan municipal." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var planesMunicipales = await _planMunicipalService.GetAllPlanesMunicipalesAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                planesMunicipales = planesMunicipales.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = planesMunicipales
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
            _logger.LogError(ex, "Error al buscar planes municipales");
            return Json(new List<object>());
        }
    }
}
