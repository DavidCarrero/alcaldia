using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;
using Proyecto_alcaldia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class SubsecretariasController : BaseController
{
    private readonly ISubsecretariaService _subsecretariaService;
    private readonly ISecretariaService _secretariaService;
    private readonly IResponsableService _responsableService;
    private readonly ILogger<SubsecretariasController> _logger;

    public SubsecretariasController(
        ISubsecretariaService subsecretariaService,
        ISecretariaService secretariaService,
        IResponsableService responsableService,
        ILogger<SubsecretariasController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _subsecretariaService = subsecretariaService;
        _secretariaService = secretariaService;
        _responsableService = responsableService;
        _logger = logger;
    }

    // GET: Subsecretarias
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        try
        {
            // Validar parámetros de paginación
            (page, pageSize) = ValidarParametrosPaginacion(page, pageSize);

            // Construir query base
            var query = _context.Subsecretarias
                .Include(s => s.SecretariasSubsecretarias)
                .ThenInclude(ss => ss.Secretaria)
                .Include(s => s.SubsecretariasResponsables)
                .ThenInclude(sr => sr.Responsable)
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
                .Select(s => new SubsecretariaViewModel
                {
                    Id = s.Id,
                    Codigo = s.Codigo,
                    Nombre = s.Nombre,
                    PresupuestoAsignado = s.PresupuestoAsignado,
                    Activo = s.Activo,
                    SecretariaId = s.SecretariasSubsecretarias.Any() 
                        ? s.SecretariasSubsecretarias.First().SecretariaId : null,
                    Responsable = s.SubsecretariasResponsables.Any() 
                        ? s.SubsecretariasResponsables.First().Responsable.NombreCompleto : ""
                })
                .ToListAsync();

            // Configurar paginación
            ConfigurarPaginacion(page, pageSize, totalCount);

            // Estadísticas
            var estadisticas = await _subsecretariaService.GetEstadisticasAsync();
            ViewBag.TotalActivas = estadisticas["TotalActivas"];
            ViewBag.ConResponsable = estadisticas["ConResponsable"];
            ViewBag.Secretarias = estadisticas["Secretarias"];

            return View(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de subsecretarías");
            TempData["Error"] = "Error al cargar las subsecretarías.";
            return View(new List<SubsecretariaViewModel>());
        }
    }

    // GET: Subsecretarias/Create
    public async Task<IActionResult> Create()
    {
        var model = new SubsecretariaViewModel
        {
            Activo = true,
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        
        // Cargar secretarías para el dropdown
        var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
        ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
        
        // Cargar responsables para el dropdown
        var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
        ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });
        
        return View("Form", model);
    }

    // POST: Subsecretarias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubsecretariaViewModel viewModel, List<int> secretariasIds, List<int> responsablesIds)
    {
        if (!ModelState.IsValid)
        {
            // Recargar dropdowns
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });
            return View("Form", viewModel);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                return View("Form", viewModel);
            }

            // Asignar alcaldía del usuario logueado
            viewModel.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Crear la subsecretaría
            var subsecretaria = await _subsecretariaService.CreateAsync(viewModel);
            
            // Crear asociaciones con secretarías
            if (secretariasIds != null && secretariasIds.Any())
            {
                foreach (var secretariaId in secretariasIds)
                {
                    var asociacion = new SecretariaSubsecretaria
                    {
                        SecretariaId = secretariaId,
                        SubsecretariaId = subsecretaria.Id,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.SecretariasSubsecretarias.Add(asociacion);
                }
                await _context.SaveChangesAsync();
            }
            
            // Crear asociaciones con responsables
            if (responsablesIds != null && responsablesIds.Any())
            {
                foreach (var responsableId in responsablesIds)
                {
                    var asociacion = new SubsecretariaResponsable
                    {
                        SubsecretariaId = subsecretaria.Id,
                        ResponsableId = responsableId,
                        FechaCreacion = DateTime.UtcNow,
                        Activo = true
                    };
                    _context.SubsecretariasResponsables.Add(asociacion);
                }
                await _context.SaveChangesAsync();
            }
            
            TempData["Success"] = $"La subsecretaría '{viewModel.Nombre}' ha sido creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la subsecretaría");
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            
            // Recargar secretarías y responsables
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });
            
            return View("Form", viewModel);
        }
    }

    // GET: Subsecretarias/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var subsecretaria = await _subsecretariaService.GetByIdAsync(id);
            if (subsecretaria == null)
            {
                TempData["Warning"] = "La subsecretaría solicitada no fue encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Cargar secretarías para el dropdown
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            
            // Cargar responsables para el dropdown
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });

            return View("Form", subsecretaria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la subsecretaría con ID {Id}", id);
            TempData["Error"] = "Error al cargar la subsecretaría.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Subsecretarias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubsecretariaViewModel viewModel, List<int> secretariasIds, List<int> responsablesIds)
    {
        if (id != viewModel.Id)
        {
            TempData["Error"] = "Los datos no coinciden.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            // Recargar dropdowns
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });
            return View("Form", viewModel);
        }

        try
        {
            await _subsecretariaService.UpdateAsync(viewModel);
            
            var deletedBy = await ObtenerUsuarioIdActual();
            
            // ============================================
            // ACTUALIZAR ASOCIACIONES CON SECRETARÍAS
            // ============================================
            // Obtener TODAS las asociaciones (incluyendo eliminadas)
            var todasAsociacionesSecretarias = await _context.SecretariasSubsecretarias
                .Where(ss => ss.SubsecretariaId == id)
                .ToListAsync();
            
            var secretariasIdsSeleccionados = secretariasIds ?? new List<int>();
            
            foreach (var asociacion in todasAsociacionesSecretarias)
            {
                if (secretariasIdsSeleccionados.Contains(asociacion.SecretariaId))
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
                    secretariasIdsSeleccionados.Remove(asociacion.SecretariaId);
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
            foreach (var secretariaId in secretariasIdsSeleccionados)
            {
                var nuevaAsociacion = new SecretariaSubsecretaria
                {
                    SecretariaId = secretariaId,
                    SubsecretariaId = id,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true,
                    IsDeleted = false
                };
                _context.SecretariasSubsecretarias.Add(nuevaAsociacion);
            }
            
            // ============================================
            // ACTUALIZAR ASOCIACIONES CON RESPONSABLES
            // ============================================
            // Obtener TODAS las asociaciones (incluyendo eliminadas)
            var todasAsociacionesResponsables = await _context.SubsecretariasResponsables
                .Where(sr => sr.SubsecretariaId == id)
                .ToListAsync();
            
            var responsablesIdsSeleccionados = responsablesIds ?? new List<int>();
            
            foreach (var asociacion in todasAsociacionesResponsables)
            {
                if (responsablesIdsSeleccionados.Contains(asociacion.ResponsableId))
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
                    responsablesIdsSeleccionados.Remove(asociacion.ResponsableId);
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
            foreach (var responsableId in responsablesIdsSeleccionados)
            {
                var nuevaAsociacion = new SubsecretariaResponsable
                {
                    SubsecretariaId = id,
                    ResponsableId = responsableId,
                    FechaCreacion = DateTime.UtcNow,
                    Activo = true,
                    IsDeleted = false
                };
                _context.SubsecretariasResponsables.Add(nuevaAsociacion);
            }
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"La subsecretaría '{viewModel.Nombre}' ha sido actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la subsecretaría con ID {Id}", id);
            var mensajeError = ObtenerMensajeErrorBaseDatos(ex);
            ModelState.AddModelError("", mensajeError);
            
            // Recargar secretarías y responsables
            var secretarias = await _secretariaService.GetAllSecretariasAsync(incluirInactivas: false);
            ViewBag.Secretarias = secretarias.Select(s => new { Id = s.Id, Text = $"{s.Codigo} - {s.Nombre}" });
            var responsables = await _responsableService.GetAllAsync(incluirInactivos: false);
            ViewBag.Responsables = responsables.Select(r => new { Id = r.Id, Text = $"{r.Id} - {r.NombreCompleto}" });
            
            return View("Form", viewModel);
        }
    }

    // POST: Subsecretarias/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var subsecretaria = await _subsecretariaService.GetByIdAsync(id);
            if (subsecretaria == null)
            {
                return Json(new { success = false, message = "La subsecretaría no fue encontrada." });
            }

            var deletedBy = await ObtenerUsuarioIdActual();
            await _subsecretariaService.DeleteAsync(id, deletedBy);
            return Json(new { success = true, message = $"La subsecretaría '{subsecretaria.Nombre}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la subsecretaría con ID {Id}", id);
            return Json(new { success = false, message = "Error al eliminar la subsecretaría." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            // Obtener el máximo ID excluyendo los eliminados con soft delete
            var maxId = await _context.Subsecretarias
                .Where(s => !s.IsDeleted)
                .Select(s => s.Id)
                .MaxAsync();
            return Json(new { nextId = maxId + 1 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener siguiente ID");
            // Si no hay registros, MaxAsync lanza excepción, retornar 1
            return Json(new { nextId = 1 });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetResponsables(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo responsables para subsecretaría ID: {Id}", id);

            // Consulta directa a la tabla intermedia con joins
            var responsables = await _context.SubsecretariasResponsables
                .Include(sr => sr.Responsable)
                    .ThenInclude(r => r.Alcaldia)
                .Include(sr => sr.Subsecretaria)
                .Where(sr => sr.SubsecretariaId == id && !sr.IsDeleted && 
                            sr.Responsable != null && sr.Responsable.Activo && !sr.Responsable.IsDeleted)
                .Select(sr => new
                {
                    Id = sr.Responsable.Id,
                    NombreCompleto = sr.Responsable.NombreCompleto,
                    NumeroIdentificacion = sr.Responsable.NumeroIdentificacion,
                    Cargo = sr.Responsable.Cargo ?? "",
                    Email = sr.Responsable.Email ?? "",
                    Telefono = sr.Responsable.Telefono ?? "",
                    TipoResponsable = sr.Responsable.TipoResponsable ?? "General",
                    TipoDocumento = sr.Responsable.TipoDocumento ?? "",
                    AlcaldiaNit = sr.Responsable.Alcaldia.Nit ?? ""
                })
                .ToListAsync();

            _logger.LogInformation("Responsables encontrados: {Count}", responsables.Count);

            return Json(responsables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener responsables de subsecretaría {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}
