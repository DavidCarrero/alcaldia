using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class DepartamentosController : BaseController
{
    private readonly IDepartamentoService _departamentoService;
    private readonly ILogger<DepartamentosController> _logger;

    public DepartamentosController(
        IDepartamentoService departamentoService,
        ILogger<DepartamentosController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _departamentoService = departamentoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Departamentos
            .Include(d => d.Municipios)
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(d => 
                d.Codigo.ToLower().Contains(searchLower) || 
                d.Nombre.ToLower().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(d => d.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DepartamentoViewModel
            {
                Id = d.Id,
                Codigo = d.Codigo,
                Nombre = d.Nombre,
                Activo = d.Activo,
                CantidadMunicipios = d.Municipios.Count
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalDepartamentos = await _context.Departamentos.CountAsync(d => !d.IsDeleted);
        ViewBag.DepartamentosActivos = await _context.Departamentos.CountAsync(d => !d.IsDeleted && d.Activo);

        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new DepartamentoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartamentoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            await _departamentoService.CreateDepartamentoAsync(model);
            TempData["Success"] = "Departamento creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear departamento");
            ModelState.AddModelError("", ex.Message);
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var departamento = await _departamentoService.GetDepartamentoByIdAsync(id);
        if (departamento == null)
        {
            return NotFound();
        }

        return View("Form", departamento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DepartamentoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            await _departamentoService.UpdateDepartamentoAsync(id, model);
            TempData["Success"] = "Departamento actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar departamento");
            ModelState.AddModelError("", ex.Message);
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var departamento = await _departamentoService.GetDepartamentoByIdAsync(id);
            if (departamento == null)
            {
                return Json(new { success = false, message = "El departamento no fue encontrado." });
            }

            await _departamentoService.DeleteDepartamentoAsync(id);
            return Json(new { success = true, message = $"El departamento '{departamento.Nombre}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar departamento");
            return Json(new { success = false, message = "Error al eliminar el departamento." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            // Solo devolver activos para dropdowns
            var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                departamentos = departamentos.Where(d => 
                    d.Codigo.ToLower().Contains(term) || 
                    d.Nombre.ToLower().Contains(term));
            }

            var result = departamentos
                .Take(limit)
                .Select(d => new {
                    id = d.Id,
                    codigo = d.Codigo,
                    nombre = d.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar departamentos");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartamentos(string searchTerm = "")
    {
        // Solo devolver departamentos activos para dropdowns
        var departamentos = string.IsNullOrEmpty(searchTerm)
            ? await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false)
            : await _departamentoService.SearchDepartamentosAsync(searchTerm);

        return Json(departamentos.Select(d => new {
            id = d.Id,
            codigo = d.Codigo,
            nombre = d.Nombre
        }));
    }

    [HttpGet]
    public async Task<IActionResult> CheckCodigo(string codigo, int? id)
    {
        var exists = await _departamentoService.CodigoExistsAsync(codigo, id);
        return Json(!exists);
    }

    // GET: Departamentos/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Departamentos
                .Where(d => !d.IsDeleted)
                .Select(d => d.Id)
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
