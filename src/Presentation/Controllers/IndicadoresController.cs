using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class IndicadoresController : BaseController
{
    private readonly IIndicadorService _indicadorService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IResponsableService _responsableService;
    private readonly IProductoService _productoService;
    private readonly ILogger<IndicadoresController> _logger;

    public IndicadoresController(
        IIndicadorService indicadorService,
        IAlcaldiaService alcaldiaService,
        IResponsableService responsableService,
        IProductoService productoService,
        ILogger<IndicadoresController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _indicadorService = indicadorService;
        _alcaldiaService = alcaldiaService;
        _responsableService = responsableService;
        _productoService = productoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Indicadores
            .Include(i => i.Responsable)
            .Include(i => i.Producto)
            .Include(i => i.Alcaldia)
            .Where(i => !i.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(i => 
                i.Codigo.ToLower().Contains(searchLower) || 
                i.Nombre.ToLower().Contains(searchLower) ||
                (i.Descripcion != null && i.Descripcion.ToLower().Contains(searchLower)));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(i => i.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new IndicadorViewModel
            {
                Id = i.Id,
                Codigo = i.Codigo,
                Nombre = i.Nombre,
                Descripcion = i.Descripcion,
                ResponsableId = i.ResponsableId,
                NombreResponsable = i.Responsable != null ? i.Responsable.NombreCompleto : "",
                ProductoId = i.ProductoId,
                NombreProducto = i.Producto != null ? i.Producto.Nombre : "",
                AlcaldiaId = i.AlcaldiaId
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalIndicadores = totalCount;
        ViewBag.TotalConProducto = await _context.Indicadores.Where(i => !i.IsDeleted && i.ProductoId != null).CountAsync();
        ViewBag.TotalConResponsable = await _context.Indicadores.Where(i => !i.IsDeleted && i.ResponsableId != null).CountAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
        ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var model = new IndicadorViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IndicadorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
            ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
                ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            model.ProductoId = NormalizarIdOpcional(model.ProductoId);
            model.ResponsableId = NormalizarIdOpcional(model.ResponsableId);
            
            await _indicadorService.CreateIndicadorAsync(model);
            TempData["Success"] = "Indicador creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear indicador");
            ModelState.AddModelError("", ex.Message);

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
            ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var indicador = await _indicadorService.GetIndicadorByIdAsync(id);
        if (indicador == null)
        {
            return NotFound();
        }

        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

        var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
        ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        return View("Form", indicador);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IndicadorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
            ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }

        try
        {
            await _indicadorService.UpdateIndicadorAsync(id, model);
            TempData["Success"] = "Indicador actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar indicador");
            ModelState.AddModelError("", ex.Message);

            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.NumeroIdentificacion} - {r.NombreCompleto}" });

            var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
            ViewBag.Productos = productos.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var indicador = await _indicadorService.GetIndicadorByIdAsync(id);
            if (indicador == null)
            {
                return Json(new { success = false, message = "El indicador no fue encontrado." });
            }

            await _indicadorService.DeleteIndicadorAsync(id);
            return Json(new { success = true, message = $"El indicador '{indicador.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar indicador");
            return Json(new { success = false, message = "Error al eliminar el indicador." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var indicadores = await _indicadorService.GetAllIndicadoresAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                indicadores = indicadores.Where(i => 
                    i.Codigo.ToLower().Contains(term) || 
                    i.Nombre.ToLower().Contains(term));
            }

            var result = indicadores
                .Take(limit)
                .Select(i => new {
                    id = i.Id,
                    codigo = i.Codigo,
                    nombre = i.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar indicadores");
            return Json(new List<object>());
        }
    }

    // GET: Indicadores/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Indicadores
                .Where(i => !i.IsDeleted)
                .Select(i => i.Id)
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
