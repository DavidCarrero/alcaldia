using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class ResponsablesController : Controller
{
    private readonly IResponsableService _responsableService;
    private readonly ILogger<ResponsablesController> _logger;

    public ResponsablesController(
        IResponsableService responsableService,
        ILogger<ResponsablesController> logger)
    {
        _responsableService = responsableService;
        _logger = logger;
    }

    // GET: Responsables
    public async Task<IActionResult> Index()
    {
        try
        {
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: true);
            var estadisticas = await _responsableService.GetEstadisticasAsync();

            ViewBag.TotalActivos = estadisticas["TotalActivos"];
            ViewBag.ConSecretaria = estadisticas["ConSecretaria"];
            ViewBag.TiposResponsable = estadisticas["TiposResponsable"];

            return View(responsables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de responsables");
            TempData["Error"] = "Error al cargar los responsables.";
            return View(new List<ResponsableViewModel>());
        }
    }

    // GET: Responsables/Create
    public IActionResult Create()
    {
        return View("Form", new ResponsableViewModel { Activo = true });
    }

    // POST: Responsables/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResponsableViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", viewModel);
        }

        try
        {
            await _responsableService.CreateAsync(viewModel);
            TempData["Success"] = $"El responsable '{viewModel.NombreCompleto}' ha sido creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el responsable");
            TempData["Error"] = "Error al crear el responsable. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // GET: Responsables/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var responsable = await _responsableService.GetByIdAsync(id);
            if (responsable == null)
            {
                TempData["Warning"] = "El responsable solicitado no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", responsable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el responsable con ID {Id}", id);
            TempData["Error"] = "Error al cargar el responsable.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Responsables/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ResponsableViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            TempData["Error"] = "Los datos no coinciden.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View("Form", viewModel);
        }

        try
        {
            await _responsableService.UpdateAsync(viewModel);
            TempData["Success"] = $"El responsable '{viewModel.NombreCompleto}' ha sido actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el responsable con ID {Id}", id);
            TempData["Error"] = "Error al actualizar el responsable. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // POST: Responsables/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var responsable = await _responsableService.GetByIdAsync(id);
            if (responsable == null)
            {
                return Json(new { success = false, message = "El responsable no fue encontrado." });
            }

            await _responsableService.DeleteAsync(id);
            return Json(new { success = true, message = $"El responsable '{responsable.NombreCompleto}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el responsable con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar el responsable." });
        }
    }
}
