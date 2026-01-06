using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class PlanesNacionalesController : BaseController
{
    private readonly IPlanNacionalService _planNacionalService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ISectorService _sectorService;
    private readonly ILogger<PlanesNacionalesController> _logger;

    public PlanesNacionalesController(
        IPlanNacionalService planNacionalService,
        IAlcaldiaService alcaldiaService,
        ISectorService sectorService,
        ILogger<PlanesNacionalesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _planNacionalService = planNacionalService;
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
        var query = _context.PlanesNacionales
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
            .Select(p => new PlanNacionalViewModel
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
        ViewBag.TotalPlanesNacionales = totalCount;
        ViewBag.TotalConSector = await _context.PlanesNacionales.Where(p => !p.IsDeleted && p.SectorId != null).CountAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        var model = new PlanNacionalViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanNacionalViewModel model)
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
            
            await _planNacionalService.CreatePlanNacionalAsync(model);
            TempData["Success"] = "Plan nacional creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plan nacional");
            ModelState.AddModelError("", ex.Message);
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var planNacional = await _planNacionalService.GetPlanNacionalByIdAsync(id);
        if (planNacional == null)
        {
            return NotFound();
        }

        var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
        ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        return View("Form", planNacional);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanNacionalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            ViewBag.Sectores = sectores.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _planNacionalService.UpdatePlanNacionalAsync(id, model);
            TempData["Success"] = "Plan nacional actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plan nacional");
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
            var planNacional = await _planNacionalService.GetPlanNacionalByIdAsync(id);
            if (planNacional == null)
            {
                return Json(new { success = false, message = "El plan nacional no fue encontrado." });
            }

            await _planNacionalService.DeletePlanNacionalAsync(id);
            return Json(new { success = true, message = $"El plan nacional '{planNacional.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar plan nacional");
            return Json(new { success = false, message = "Error al eliminar el plan nacional." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var planesNacionales = await _planNacionalService.GetAllPlanesNacionalesAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                planesNacionales = planesNacionales.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = planesNacionales
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
            _logger.LogError(ex, "Error al buscar planes nacionales");
            return Json(new List<object>());
        }
    }

    // GET: PlanesNacionales/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.PlanesNacionales
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
