using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class MunicipiosController : BaseController
{
    private readonly IMunicipioService _municipioService;
    private readonly IDepartamentoService _departamentoService;
    private readonly ILogger<MunicipiosController> _logger;

    public MunicipiosController(
        IMunicipioService municipioService,
        IDepartamentoService departamentoService,
        ILogger<MunicipiosController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _municipioService = municipioService;
        _departamentoService = departamentoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.Municipios
            .Include(m => m.Departamentos)
            .Where(m => !m.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(m => 
                m.Codigo.ToLower().Contains(searchLower) || 
                m.Nombre.ToLower().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(m => m.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MunicipioViewModel
            {
                Id = m.Id,
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                Activo = m.Activo,
                NombreDepartamento = m.Departamentos.Any() 
                    ? m.Departamentos.First().Nombre : "",
                DepartamentoIds = m.Departamentos.Select(d => d.Id).ToList()
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas
        ViewBag.TotalMunicipios = await _context.Municipios.CountAsync(m => !m.IsDeleted);
        ViewBag.MunicipiosActivos = await _context.Municipios.CountAsync(m => !m.IsDeleted && m.Activo);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
        ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
        return View("Form", new MunicipioViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MunicipioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
            ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _municipioService.CreateMunicipioAsync(model);
            TempData["Success"] = "Municipio creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear municipio");
            ModelState.AddModelError("", ex.Message);
            var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
            ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var municipio = await _municipioService.GetMunicipioByIdAsync(id);
        if (municipio == null)
        {
            return NotFound();
        }

        var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
        ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
        return View("Form", municipio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MunicipioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
            ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _municipioService.UpdateMunicipioAsync(id, model);
            TempData["Success"] = "Municipio actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar municipio");
            ModelState.AddModelError("", ex.Message);
            var departamentos = await _departamentoService.GetAllDepartamentosAsync(incluirInactivos: false);
            ViewBag.Departamentos = departamentos.Select(d => new { Id = d.Id, Text = $"{d.Codigo} - {d.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var municipio = await _municipioService.GetMunicipioByIdAsync(id);
            if (municipio == null)
            {
                return Json(new { success = false, message = "El municipio no fue encontrado." });
            }

            await _municipioService.DeleteMunicipioAsync(id);
            return Json(new { success = true, message = $"El municipio '{municipio.Nombre}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar municipio");
            return Json(new { success = false, message = "Error al eliminar el municipio." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            // Solo devolver activos para dropdowns
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                municipios = municipios.Where(m => 
                    m.Codigo.ToLower().Contains(term) || 
                    m.Nombre.ToLower().Contains(term));
            }

            var result = municipios
                .Take(limit)
                .Select(m => new {
                    id = m.Id,
                    codigo = m.Codigo,
                    nombre = m.Nombre,
                    departamento = m.NombreDepartamento
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar municipios");
            return Json(new List<object>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMunicipios(string searchTerm = "")
    {
        // Solo devolver municipios activos para dropdowns
        var municipios = string.IsNullOrEmpty(searchTerm)
            ? await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false)
            : await _municipioService.SearchMunicipiosAsync(searchTerm);

        return Json(municipios.Select(m => new {
            id = m.Id,
            codigo = m.Codigo,
            nombre = m.Nombre,
            departamento = m.NombreDepartamento
        }));
    }

    // GET: Municipios/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Municipios
                .Where(m => !m.IsDeleted)
                .Select(m => m.Id)
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
