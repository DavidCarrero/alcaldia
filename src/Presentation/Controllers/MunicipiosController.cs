using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Index()
    {
        var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: true);
        return View(municipios);
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
}
