using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        
        ViewBag.TotalMetas = totalMetas;

        return View(ods);
    }

    public async Task<IActionResult> Create()
    {
        await CargarDatosFormulario();
        
        var model = new ODSViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ODSViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Validar que el usuario tenga una alcaldía asignada
                if (!ValidarAlcaldiaId())
                {
                    return View(model);
                }

                // Asignar alcaldía del usuario logueado
                model.AlcaldiaId = ObtenerAlcaldiaId();
                
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
        // Cargar todas las metas ODS
        var metasODS = await _metaODSService.GetAllMetasODSAsync();
        ViewBag.MetasODS = metasODS.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" }).ToList();
    }
}
