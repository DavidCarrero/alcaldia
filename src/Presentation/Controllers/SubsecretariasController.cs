using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class SubsecretariasController : BaseController
{
    private readonly ISubsecretariaService _subsecretariaService;
    private readonly ILogger<SubsecretariasController> _logger;

    public SubsecretariasController(
        ISubsecretariaService subsecretariaService,
        ILogger<SubsecretariasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _subsecretariaService = subsecretariaService;
        _logger = logger;
    }

    // GET: Subsecretarias
    public async Task<IActionResult> Index()
    {
        try
        {
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: true);
            var estadisticas = await _subsecretariaService.GetEstadisticasAsync();

            ViewBag.TotalActivas = estadisticas["TotalActivas"];
            ViewBag.ConResponsable = estadisticas["ConResponsable"];
            ViewBag.Secretarias = estadisticas["Secretarias"];

            return View(subsecretarias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de subsecretarías");
            TempData["Error"] = "Error al cargar las subsecretarías.";
            return View(new List<SubsecretariaViewModel>());
        }
    }

    // GET: Subsecretarias/Create
    public IActionResult Create()
    {
        var model = new SubsecretariaViewModel
        {
            Activo = true,
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    // POST: Subsecretarias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubsecretariaViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", viewModel);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                return View("Form", viewModel);
            }

            // Asignar alcaldía del usuario logueado
            viewModel.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            viewModel.SecretariaId = NormalizarIdOpcional(viewModel.SecretariaId);
            
            await _subsecretariaService.CreateAsync(viewModel);
            TempData["Success"] = $"La subsecretaría '{viewModel.Nombre}' ha sido creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la subsecretaría");
            TempData["Error"] = "Error al crear la subsecretaría. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // GET: Subsecretarias/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var subsecretaria = await _subsecretariaService.GetByIdAsync(id);
            if (subsecretaria == null)
            {
                TempData["Warning"] = "La subsecretaría solicitada no fue encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View("Form", subsecretaria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la subsecretaría con ID {Id}", id);
            TempData["Error"] = "Error al cargar la subsecretaría.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Subsecretarias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubsecretariaViewModel viewModel)
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
            await _subsecretariaService.UpdateAsync(viewModel);
            TempData["Success"] = $"La subsecretaría '{viewModel.Nombre}' ha sido actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la subsecretaría con ID {Id}", id);
            TempData["Error"] = "Error al actualizar la subsecretaría. Por favor, intente nuevamente.";
            return View("Form", viewModel);
        }
    }

    // POST: Subsecretarias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var subsecretaria = await _subsecretariaService.GetByIdAsync(id);
            if (subsecretaria == null)
            {
                return Json(new { success = false, message = "La subsecretaría no fue encontrada." });
            }

            await _subsecretariaService.DeleteAsync(id);
            return Json(new { success = true, message = $"La subsecretaría '{subsecretaria.Nombre}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la subsecretaría con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar la subsecretaría." });
        }
    }
}
