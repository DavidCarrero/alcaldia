using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;
using Proyecto_alcaldia.Presentation.Controllers;

namespace Proyecto_alcaldia.Controllers;

public class ODSController : BaseController
{
    private readonly IODSService _odsService;
    private readonly IMetaODSService _metaODSService;

    public ODSController(
        ApplicationDbContext context,
        IServiceProvider serviceProvider,
        IODSService odsService,
        IMetaODSService metaODSService) : base(context, serviceProvider)
    {
        _odsService = odsService;
        _metaODSService = metaODSService;
    }

    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

        // Construir query base
        var query = _context.ODS
            .Include(o => o.ODSMetasODS)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(o => 
                o.Codigo.ToLower().Contains(searchLower) || 
                o.Nombre.ToLower().Contains(searchLower));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar
        query = query.OrderBy(o => o.Codigo);

        // Contar total
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        // Aplicar paginación
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new ODSViewModel
            {
                Id = o.Id,
                Codigo = o.Codigo,
                Nombre = o.Nombre,
                Descripcion = o.Descripcion,
                FechaInicio = o.FechaInicio,
                FechaFin = o.FechaFin,
                Activo = o.Activo,
                MetasODSIds = o.ODSMetasODS.Select(m => m.MetaODSId).ToList()
            })
            .ToListAsync();

        // Configurar paginación
        ConfigurarPaginacion(page, pageSize, totalCount);

        // Estadísticas (del total, no de la página)
        var totalODS = await _context.ODS.CountAsync(o => !o.IsDeleted);
        var odsActivos = await _context.ODS.CountAsync(o => !o.IsDeleted && o.Activo);
        var totalMetas = await _context.MetasODS.CountAsync(m => !m.IsDeleted);
        
        ViewBag.TotalODS = totalODS;
        ViewBag.ODSActivos = odsActivos;
        ViewBag.ODSInactivos = totalODS - odsActivos;
        ViewBag.TotalMetas = totalMetas;

        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        await CargarDatosFormulario();
        
        var model = new ODSViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ODSViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Validar que el usuario tenga una alcaldía asignada
                if (!ValidarAlcaldiaId())
                {
                    return View(model);
                }

                // Asignar alcaldía del usuario logueado
                model.AlcaldiaId = ObtenerAlcaldiaId();
                
                await _odsService.CreateODSAsync(model);
                TempData["Success"] = "ODS creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear el ODS: {ex.Message}";
            }
        }

        await CargarDatosFormulario();
        return View("Form", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ods = await _odsService.GetODSByIdAsync(id);
        if (ods == null)
        {
            TempData["Error"] = "ODS no encontrado";
            return RedirectToAction(nameof(Index));
        }

        await CargarDatosFormulario();
        return View("Form", ods);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ODSViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _odsService.UpdateODSAsync(id, model);
                TempData["Success"] = "ODS actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar el ODS: {ex.Message}";
            }
        }

        await CargarDatosFormulario();
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _odsService.DeleteODSAsync(id);
            TempData["Success"] = "ODS eliminado exitosamente";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al eliminar el ODS: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDatosFormulario()
    {
        // Cargar todas las metas ODS
        var metasODS = await _metaODSService.GetAllMetasODSAsync();
        ViewBag.MetasODS = metasODS.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> GetNextCodigo()
    {
        try
        {
            var maxId = await _context.ODS
                .MaxAsync(o => (int?)o.Id) ?? 0;
            
            return Json(new { success = true, codigo = (maxId + 1).ToString() });
        }
        catch (Exception)
        {
            return Json(new { success = true, codigo = "1" });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.ODS
                .MaxAsync(o => (int?)o.Id) ?? 0;
            
            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception)
        {
            return Json(new { nextId = 1 });
        }
    }
}
