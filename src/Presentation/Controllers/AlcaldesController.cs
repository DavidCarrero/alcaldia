using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class AlcaldesController : BaseController
{
    private readonly IAlcaldeService _alcaldeService;
    private readonly ILogger<AlcaldesController> _logger;

    public AlcaldesController(
        IAlcaldeService alcaldeService,
        ILogger<AlcaldesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _alcaldeService = alcaldeService;
        _logger = logger;
    }

    // GET: Alcaldes
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        try
        {
            // Validar parámetros de paginación
            (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

            // Construir query base
            var query = _context.Alcaldes
                .Include(a => a.Alcaldia)
                .Include(a => a.AlcaldesVigencias)
                .ThenInclude(av => av.Vigencia)
                .Where(a => !a.IsDeleted)
                .AsQueryable();

            // Aplicar búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(a => 
                    a.NombreCompleto.ToLower().Contains(searchLower) || 
                    a.NumeroDocumento.ToLower().Contains(searchLower) ||
                    (a.PartidoPolitico != null && a.PartidoPolitico.ToLower().Contains(searchLower)));
                ViewData["SearchTerm"] = searchTerm;
            }

            // Ordenar
            query = query.OrderByDescending(a => a.Id);

            // Contar total
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Min(page, Math.Max(1, totalPages));

            // Aplicar paginación
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlcaldeViewModel
                {
                    Id = a.Id,
                    NombreCompleto = a.NombreCompleto,
                    NumeroDocumento = a.NumeroDocumento,
                    PartidoPolitico = a.PartidoPolitico,
                    PeriodoInicio = a.PeriodoInicio,
                    PeriodoFin = a.PeriodoFin,
                    Activo = a.Activo,
                    AlcaldiaId = a.AlcaldiaId,
                    NitAlcaldia = a.Alcaldia != null ? a.Alcaldia.Nit : ""
                })
                .ToListAsync();

            // Configurar paginación
            ConfigurarPaginacion(page, pageSize, totalCount);

            // Estadísticas
            var estadisticas = await _alcaldeService.GetEstadisticasAsync();
            ViewBag.TotalActivos = estadisticas["TotalActivos"];
            ViewBag.EnPeriodo = estadisticas["EnPeriodo"];
            ViewBag.Partidos = estadisticas["Partidos"];

            return View(items);
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
        var model = new AlcaldeViewModel
        {
            Activo = true,
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
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
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                TempData["Error"] = "No tiene una alcaldía asignada.";
                return View("Form", viewModel);
            }

            // Asignar alcaldía del usuario logueado
            viewModel.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _alcaldeService.CreateAsync(viewModel);
            TempData["Success"] = $"El alcalde '{viewModel.NombreCompleto}' ha sido creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el alcalde");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
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
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
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

            var deletedBy = await ObtenerUsuarioIdActual();
            await _alcaldeService.DeleteAsync(id, deletedBy);
            return Json(new { success = true, message = $"El alcalde '{alcalde.NombreCompleto}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el alcalde con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar el alcalde." });
        }
    }

    // GET: Alcaldes/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Alcaldes
                .Where(a => !a.IsDeleted)
                .Select(a => a.Id)
                .DefaultIfEmpty(0)
                .MaxAsync();

            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception)
        {
            return Json(new { nextId = 1 });
        }
    }
}
