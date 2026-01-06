using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class ResponsablesController : BaseController
{
    private readonly IResponsableService _responsableService;
    private readonly ISubsecretariaService _subsecretariaService;
    private readonly ILogger<ResponsablesController> _logger;

    public ResponsablesController(
        IResponsableService responsableService,
        ISubsecretariaService subsecretariaService,
        ILogger<ResponsablesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _responsableService = responsableService;
        _subsecretariaService = subsecretariaService;
        _logger = logger;
    }

    // GET: Responsables
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        try
        {
            // Validar parámetros de paginación
            (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

            // Construir query base
            var query = _context.Responsables
                .Include(r => r.SubsecretariasResponsables)
                .ThenInclude(sr => sr.Subsecretaria)
                .Include(r => r.Alcaldia)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            // Aplicar búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(r => 
                    r.NombreCompleto.ToLower().Contains(searchLower) || 
                    r.NumeroIdentificacion.ToLower().Contains(searchLower) ||
                    (r.Cargo != null && r.Cargo.ToLower().Contains(searchLower)) ||
                    (r.Email != null && r.Email.ToLower().Contains(searchLower)));
                ViewData["SearchTerm"] = searchTerm;
            }

            // Ordenar
            query = query.OrderBy(r => r.NumeroIdentificacion);

            // Contar total
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Min(page, Math.Max(1, totalPages));

            // Aplicar paginación
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ResponsableViewModel
                {
                    Id = r.Id,
                    NumeroIdentificacion = r.NumeroIdentificacion,
                    NombreCompleto = r.NombreCompleto,
                    Cargo = r.Cargo,
                    Email = r.Email,
                    Telefono = r.Telefono,
                    Activo = r.Activo,
                    AlcaldiaId = r.AlcaldiaId
                })
                .ToListAsync();

            // Configurar paginación
            ConfigurarPaginacion(page, pageSize, totalCount);

            // Estadísticas
            var estadisticas = await _responsableService.GetEstadisticasAsync();
            ViewBag.TotalActivos = estadisticas["TotalActivos"];
            ViewBag.ConSecretaria = estadisticas["ConSecretaria"];
            ViewBag.TiposResponsable = estadisticas["TiposResponsable"];

            return View(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de responsables");
            TempData["Error"] = "Error al cargar los responsables.";
            return View(new List<ResponsableViewModel>());
        }
    }

    // GET: Responsables/Create
    public async Task<IActionResult> Create()
    {
        var model = new ResponsableViewModel
        {
            Activo = true,
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };

        // Cargar subsecretarías para el dropdown
        var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
        ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

        return View("Form", model);
    }

    // POST: Responsables/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResponsableViewModel viewModel, List<int>? subsecretariasIds)
    {
        if (!ModelState.IsValid)
        {
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
            ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", viewModel);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
                ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
                return View("Form", viewModel);
            }

            // Asignar alcaldía del usuario logueado
            viewModel.AlcaldiaId = ObtenerAlcaldiaId();

            // Crear el responsable
            var responsable = await _responsableService.CreateAsync(viewModel);

            // Crear asociaciones con subsecretarías
            if (subsecretariasIds != null && subsecretariasIds.Any())
            {
                foreach (var subsecretariaId in subsecretariasIds)
                {
                    var asociacion = new SubsecretariaResponsable
                    {
                        SubsecretariaId = subsecretariaId,
                        ResponsableId = responsable.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.SubsecretariasResponsables.Add(asociacion);
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"El responsable '{viewModel.NombreCompleto}' ha sido creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el responsable");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);

            // Recargar subsecretarías
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
            ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            return View("Form", viewModel);
        }
    }

    // GET: Responsables/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var responsable = await _responsableService.GetByIdAsync(id);
            if (responsable == null)
            {
                TempData["Warning"] = "El responsable solicitado no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Cargar subsecretarías para el dropdown
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
            ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            // Cargar subsecretarías asociadas
            var subsecretariasAsociadas = await _context.SubsecretariasResponsables
                .Where(sr => sr.ResponsableId == id && !sr.IsDeleted)
                .Select(sr => sr.SubsecretariaId)
                .ToListAsync();

            ViewBag.SubsecretariasIds = subsecretariasAsociadas;

            return View("Form", responsable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el responsable con ID {Id}", id);
            TempData["Error"] = "Error al cargar el responsable.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Responsables/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ResponsableViewModel viewModel, List<int>? subsecretariasIds)
    {
        if (id != viewModel.Id)
        {
            TempData["Error"] = "Los datos no coinciden.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
            ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            return View("Form", viewModel);
        }

        try
        {
            await _responsableService.UpdateAsync(viewModel);

            var deletedBy = await ObtenerUsuarioIdActual();
            
            // ============================================
            // ACTUALIZAR ASOCIACIONES CON SUBSECRETARÍAS
            // ============================================
            // Obtener TODAS las asociaciones (incluyendo eliminadas)
            var todasAsociaciones = await _context.SubsecretariasResponsables
                .Where(sr => sr.ResponsableId == id)
                .ToListAsync();

            var subsecretariasIdsSeleccionados = subsecretariasIds ?? new List<int>();

            foreach (var asociacion in todasAsociaciones)
            {
                if (subsecretariasIdsSeleccionados.Contains(asociacion.SubsecretariaId))
                {
                    // La relación debe existir - restaurar si estaba eliminada
                    if (asociacion.IsDeleted)
                    {
                        asociacion.IsDeleted = false;
                        asociacion.DeletedAt = null;
                        asociacion.DeletedBy = null;
                        asociacion.FechaActualizacion = DateTime.UtcNow;
                    }
                    // Remover de la lista para saber cuáles son nuevas
                    subsecretariasIdsSeleccionados.Remove(asociacion.SubsecretariaId);
                }
                else
                {
                    // La relación NO debe existir - eliminarla si está activa
                    if (!asociacion.IsDeleted)
                    {
                        asociacion.IsDeleted = true;
                        asociacion.DeletedAt = DateTime.UtcNow;
                        asociacion.DeletedBy = deletedBy;
                    }
                }
            }

            // Crear las asociaciones completamente nuevas
            foreach (var subsecretariaId in subsecretariasIdsSeleccionados)
            {
                var nuevaAsociacion = new SubsecretariaResponsable
                {
                    SubsecretariaId = subsecretariaId,
                    ResponsableId = id,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true,
                    IsDeleted = false
                };
                _context.SubsecretariasResponsables.Add(nuevaAsociacion);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"El responsable '{viewModel.NombreCompleto}' ha sido actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el responsable con ID {Id}", id);
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);

            // Recargar subsecretarías
            var subsecretarias = await _subsecretariaService.GetAllAsync(incluirInactivos: false);
            ViewBag.Subsecretarias = subsecretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });

            return View("Form", viewModel);
        }
    }

    // POST: Responsables/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var responsable = await _responsableService.GetByIdAsync(id);
            if (responsable == null)
            {
                return Json(new { success = false, message = "El responsable no fue encontrado." });
            }

            var deletedBy = await ObtenerUsuarioIdActual();
            await _responsableService.DeleteAsync(id, deletedBy);
            return Json(new { success = true, message = $"El responsable '{responsable.NombreCompleto}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el responsable con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar el responsable." });
        }
    }

    // GET: Responsables/Search
    [HttpGet]
    public async Task<IActionResult> Search(string searchTerm)
    {
        try
        {
            var responsables = string.IsNullOrWhiteSpace(searchTerm)
                ? await _responsableService.GetAllAsync(incluirInactivos: true)
                : await _responsableService.SearchAsync(searchTerm);

            return PartialView("_ResponsablesTable", responsables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar responsables con el término: {SearchTerm}", searchTerm);
            return PartialView("_ResponsablesTable", new List<ResponsableViewModel>());
        }
    }

    // GET: Responsables/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            // Obtener el máximo ID excluyendo los eliminados con soft delete
            var maxId = await _context.Responsables
                .Where(r => !r.IsDeleted)
                .Select(r => r.Id)
                .MaxAsync();

            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception ex)
        {
            // Si no hay registros, devolver 1
            return Json(new { nextId = 1 });
        }
    }
}
