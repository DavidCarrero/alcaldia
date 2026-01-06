using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class SectoresController : BaseController
{
    private readonly ISectorService _sectorService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly ILineaEstrategicaService _lineaEstrategicaService;
    private readonly ILogger<SectoresController> _logger;

    public SectoresController(
        ISectorService sectorService,
        IAlcaldiaService alcaldiaService,
        ILineaEstrategicaService lineaEstrategicaService,
        ILogger<SectoresController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _sectorService = sectorService;
        _alcaldiaService = alcaldiaService;
        _lineaEstrategicaService = lineaEstrategicaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Sectores
            .Include(s => s.LineaEstrategica)
            .Include(s => s.Programas)
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(s => 
                s.Codigo.ToLower().Contains(searchLower) || 
                s.Nombre.ToLower().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(s => s.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SectorViewModel
            {
                Id = s.Id,
                Codigo = s.Codigo,
                Nombre = s.Nombre,
                Descripcion = s.Descripcion,
                PresupuestoAsignado = s.PresupuestoAsignado,
                Activo = s.Activo,
                LineaEstrategicaId = s.LineaEstrategicaId,
                NombreLineaEstrategica = s.LineaEstrategica != null ? s.LineaEstrategica.Nombre : "",
                CantidadProgramas = s.Programas.Count(p => !p.IsDeleted)
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalSectores = await _context.Sectores.CountAsync(s => !s.IsDeleted);
        ViewBag.SectoresActivos = await _context.Sectores.CountAsync(s => !s.IsDeleted && s.Activo);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
        ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });

        var model = new SectorViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SectorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
                ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            model.LineaEstrategicaId = NormalizarIdOpcional(model.LineaEstrategicaId);
            
            await _sectorService.CreateSectorAsync(model);
            TempData["Success"] = "Sector creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sector");
            ModelState.AddModelError("", ex.Message);
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sector = await _sectorService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }

        var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
        ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });

        return View("Form", sector);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SectorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _sectorService.UpdateSectorAsync(id, model);
            TempData["Success"] = "Sector actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sector");
            ModelState.AddModelError("", ex.Message);
            var lineasEstrategicas = await _lineaEstrategicaService.GetAllLineasEstrategicasAsync(incluirInactivas: false);
            ViewBag.LineasEstrategicas = lineasEstrategicas.Select(l => new { Id = l.Id, Text = $"{l.Codigo} - {l.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sector = await _sectorService.GetSectorByIdAsync(id);
            if (sector == null)
            {
                return Json(new { success = false, message = "El sector no fue encontrado." });
            }

            await _sectorService.DeleteSectorAsync(id);
            return Json(new { success = true, message = $"El sector '{sector.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar sector");
            return Json(new { success = false, message = "Error al eliminar el sector." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var sectores = await _sectorService.GetAllSectoresAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                sectores = sectores.Where(s => 
                    s.Codigo.ToLower().Contains(term) || 
                    s.Nombre.ToLower().Contains(term));
            }

            var result = sectores
                .Take(limit)
                .Select(s => new {
                    id = s.Id,
                    codigo = s.Codigo,
                    nombre = s.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar sectores");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNextCodigo()
    {
        try
        {
            var maxId = await _context.Sectores
                .MaxAsync(s => (int?)s.Id) ?? 0;
            
            return Json(new { nextCodigo = (maxId + 1).ToString() });
        }
        catch (Exception)
        {
            return Json(new { nextCodigo = "1" });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Sectores
                .MaxAsync(s => (int?)s.Id) ?? 0;
            
            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception)
        {
            return Json(new { nextId = 1 });
        }
    }
}
