using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;
using Proyecto_alcaldia.Presentation.Controllers;

namespace Proyecto_alcaldia.Controllers;

public class ODSController : BaseController
{
    private readonly IODSService _odsService;
    private readonly IMetaODSService _metaODSService;

    public ODSController(
        ApplicationDbContext context,
        IServiceProvider serviceProvider,
        IODSService odsService,
        IMetaODSService metaODSService) : base(context, serviceProvider)
    {
        _odsService = odsService;
        _metaODSService = metaODSService;
    }

    public async Task<IActionResult> Index(string? searchTerm = null)
    {
        IEnumerable<ODSViewModel> ods;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            ods = await _odsService.SearchODSAsync(searchTerm);
            ViewData["SearchTerm"] = searchTerm;
        }
        else
        {
            ods = await _odsService.GetAllODSAsync();
        }

        // Estadísticas
        var todosODS = await _odsService.GetAllODSAsync(incluirInactivos: true);
        ViewBag.TotalODS = todosODS.Count();
        ViewBag.ODSActivos = todosODS.Count(o => o.Activo);
        ViewBag.ODSInactivos = todosODS.Count(o => !o.Activo);
        
        // KPIs válidos
        var todasMetasODS = await _metaODSService.GetAllMetasODSAsync();
        var totalMetas = todasMetasODS.Count();
        var metasActivas = todasMetasODS.Count(m => m.Activo);
        
        ViewBag.TotalMetas = totalMetas;
        ViewBag.MetasActivas = metasActivas;
        ViewBag.ImpactoAlto = todosODS.Count(o => o.NivelImpacto == "Alta");
        ViewBag.ImpactoMedio = todosODS.Count(o => o.NivelImpacto == "Media");

        return View(ods);
    }

    public async Task<IActionResult> Create()
    {
        await CargarDatosFormulario();
        return View("Form", new ODSViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ODSViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _odsService.CreateODSAsync(model);
                TempData["Success"] = "ODS creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear el ODS: {ex.Message}";
            }
        }

        await CargarDatosFormulario();
        return View("Form", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ods = await _odsService.GetODSByIdAsync(id);
        if (ods == null)
        {
            TempData["Error"] = "ODS no encontrado";
            return RedirectToAction(nameof(Index));
        }

        await CargarDatosFormulario();
        return View("Form", ods);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ODSViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _odsService.UpdateODSAsync(id, model);
                TempData["Success"] = "ODS actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar el ODS: {ex.Message}";
            }
        }

        await CargarDatosFormulario();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _odsService.DeleteODSAsync(id);
            TempData["Success"] = "ODS eliminado exitosamente";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al eliminar el ODS: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDatosFormulario()
    {
        // Cargar alcaldías disponibles
        var alcaldias = _context.Alcaldias
            .Where(a => a.Activo)
            .OrderBy(a => a.Nit)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Nit} - {a.Municipio.Nombre}"
            })
            .ToList();

        ViewBag.Alcaldias = alcaldias;

        // Cargar todas las metas ODS
        var metasODS = await _metaODSService.GetAllMetasODSAsync();
        ViewBag.MetasODS = metasODS.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" }).ToList();

        // Niveles de impacto
        ViewBag.NivelesImpacto = new List<SelectListItem>
        {
            new SelectListItem { Value = "Alta", Text = "Alta" },
            new SelectListItem { Value = "Media", Text = "Media" },
            new SelectListItem { Value = "Baja", Text = "Baja" }
        };

        // Estados
        ViewBag.Estados = new List<SelectListItem>
        {
            new SelectListItem { Value = "ACTIVO", Text = "Activo" },
            new SelectListItem { Value = "INACTIVO", Text = "Inactivo" }
        };
    }
}
