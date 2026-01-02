using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class AlcaldesController : Controller
{
    private readonly IAlcaldeService _alcaldeService;
    private readonly ILogger<AlcaldesController> _logger;

    public AlcaldesController(
        IAlcaldeService alcaldeService,
        ILogger<AlcaldesController> logger)
    {
        _alcaldeService = alcaldeService;
        _logger = logger;
    }

    // GET: Alcaldes
    public async Task<IActionResult> Index()
    {
        try
        {
            var alcaldes = await _alcaldeService.GetAllAsync(incluirInactivos: true);
            var estadisticas = await _alcaldeService.GetEstadisticasAsync();

            ViewBag.TotalActivos = estadisticas["TotalActivos"];
            ViewBag.EnPeriodo = estadisticas["EnPeriodo"];
            ViewBag.Partidos = estadisticas["Partidos"];

            return View(alcaldes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de alcaldes");
            TempData["Error"] = "Error al cargar los alcaldes.";
            return View(new List<AlcaldeViewModel>());
        }
    }

    // GET: Alcaldes/Create
    public IActionResult Create()
    {
        return View("Form", new AlcaldeViewModel { Activo = true });
    }

    // POST: Alcaldes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlcaldeViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", viewModel);
        }

        try
        {
            await _alcaldeService.CreateAsync(viewModel);
            TempData["Success"] = $"El alcalde '{viewModel.NombreCompleto}' ha sido creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el alcalde");
            TempData["Error"] = "Error al crear el alcalde. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // GET: Alcaldes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var alcalde = await _alcaldeService.GetByIdAsync(id);
            if (alcalde == null)
            {
                TempData["Warning"] = "El alcalde solicitado no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", alcalde);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el alcalde con ID {Id}", id);
            TempData["Error"] = "Error al cargar el alcalde.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Alcaldes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlcaldeViewModel viewModel)
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
            await _alcaldeService.UpdateAsync(viewModel);
            TempData["Success"] = $"El alcalde '{viewModel.NombreCompleto}' ha sido actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el alcalde con ID {Id}", id);
            TempData["Error"] = "Error al actualizar el alcalde. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // POST: Alcaldes/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var alcalde = await _alcaldeService.GetByIdAsync(id);
            if (alcalde == null)
            {
                return Json(new { success = false, message = "El alcalde no fue encontrado." });
            }

            await _alcaldeService.DeleteAsync(id);
            return Json(new { success = true, message = $"El alcalde '{alcalde.NombreCompleto}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el alcalde con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar el alcalde." });
        }
    }
}
