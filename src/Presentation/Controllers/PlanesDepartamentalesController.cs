using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class PlanesDepartamentalesController : BaseController
{
    private readonly IPlanDepartamentalService _planDepartamentalService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ISectorService _sectorService;
    private readonly ILogger<PlanesDepartamentalesController> _logger;

    public PlanesDepartamentalesController(
        IPlanDepartamentalService planDepartamentalService,
        IAlcaldiaService alcaldiaService,
        ISectorService sectorService,
        ILogger<PlanesDepartamentalesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _planDepartamentalService = planDepartamentalService;
        _alcaldiaService = alcaldiaService;
        _sectorService = sectorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.PlanesDepartamentales
            .Include(p => p.Sector)
            .Include(p => p.Alcaldia)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(p => 
                p.Codigo.ToLower().Contains(searchLower) || 
                p.Nombre.ToLower().Contains(searchLower) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(searchLower)));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(p => p.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PlanDepartamentalViewModel
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                SectorId = p.SectorId,
                NombreSector = p.Sector != null ? p.Sector.Nombre : "",
                AlcaldiaId = p.AlcaldiaId,
                NitAlcaldia = p.Alcaldia != null ? p.Alcaldia.Nit : ""
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalPlanesDepartamentales = totalCount;
        ViewBag.TotalConSector = await _context.PlanesDepartamentales.Where(p => !p.IsDeleted && p.SectorId != null).CountAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        var model = new PlanDepartamentalViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanDepartamentalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
                ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            await _planDepartamentalService.CreatePlanDepartamentalAsync(model);
            TempData["Success"] = "Plan departamental creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plan departamental");
            ModelState.AddModelError("", ex.Message);
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var planDepartamental = await _planDepartamentalService.GetPlanDepartamentalByIdAsync(id);
        if (planDepartamental == null)
        {
            return NotFound();
        }

        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        return View("Form", planDepartamental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanDepartamentalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _planDepartamentalService.UpdatePlanDepartamentalAsync(id, model);
            TempData["Success"] = "Plan departamental actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plan departamental");
            ModelState.AddModelError("", ex.Message);
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var planDepartamental = await _planDepartamentalService.GetPlanDepartamentalByIdAsync(id);
            if (planDepartamental == null)
            {
                return Json(new { success = false, message = "El plan departamental no fue encontrado." });
            }

            await _planDepartamentalService.DeletePlanDepartamentalAsync(id);
            return Json(new { success = true, message = $"El plan departamental '{planDepartamental.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar plan departamental");
            return Json(new { success = false, message = "Error al eliminar el plan departamental." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var planesDepartamentales = await _planDepartamentalService.GetAllPlanesDepartamentalesAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                planesDepartamentales = planesDepartamentales.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = planesDepartamentales
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
            _logger.LogError(ex, "Error al buscar planes departamentales");
            return Json(new List<object>());
        }
    }

    // GET: PlanesDepartamentales/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.PlanesDepartamentales
                .Where(p => !p.IsDeleted)
                .Select(p => p.Id)
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
