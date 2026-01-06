using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;
using Proyecto_alcaldia.Presentation.Models;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class AlcaldiasController : BaseController
{
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IMunicipioService _municipioService;
    private readonly ILogger<AlcaldiasController> _logger;
    private readonly IWebHostEnvironment _env;

    public AlcaldiasController(
        IAlcaldiaService alcaldiaService,
        IMunicipioService municipioService,
        ILogger<AlcaldiasController> logger,
        IWebHostEnvironment env,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _alcaldiaService = alcaldiaService;
        _municipioService = municipioService;
        _logger = logger;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = null, int page = 1, int pageSize = 5)
    {
        // Validar parámetros de paginación
        page = Math.Max(1, page);
        pageSize = new[] { 5, 10, 20, 50, 100 }.Contains(pageSize) ? pageSize : 5;

        // Construir query base con paginación del lado del servidor
        var query = _context.Alcaldias
            .Include(a => a.Municipio)
            .ThenInclude(m => m!.Departamentos)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        // Aplicar búsqueda si existe
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(a => 
                a.Nit.ToLower().Contains(searchLower) || 
                (a.Municipio != null && a.Municipio.Nombre.ToLower().Contains(searchLower)));
            ViewData["SearchTerm"] = searchTerm;
        }

        // Ordenar por ID
        query = query.OrderByDescending(a => a.Id);

        // Obtener total de registros para estadísticas (sin paginación)
        var totalAlcaldias = await _context.Alcaldias.CountAsync(a => !a.IsDeleted);
        var alcaldiasActivas = await _context.Alcaldias.CountAsync(a => !a.IsDeleted && a.Activo);

        // Aplicar paginación
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Min(page, Math.Max(1, totalPages));

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AlcaldiaViewModel
            {
                Id = a.Id,
                Nit = a.Nit,
                Logo = a.Logo,
                Telefono = a.Telefono,
                CorreoInstitucional = a.CorreoInstitucional,
                MunicipioId = a.MunicipioId ?? 0,
                NombreMunicipio = a.Municipio != null ? a.Municipio.Nombre : "",
                NombreDepartamento = a.Municipio != null && a.Municipio.Departamentos.Any() 
                    ? a.Municipio.Departamentos.First().Nombre : "",
                Activo = a.Activo
            })
            .ToListAsync();

        // Configurar ViewBag para paginación
        ViewBag.PageIndex = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.FirstItemIndex = totalCount > 0 ? (page - 1) * pageSize + 1 : 0;
        ViewBag.LastItemIndex = Math.Min(page * pageSize, totalCount);
        ViewBag.HasPreviousPage = page > 1;
        ViewBag.HasNextPage = page < totalPages;

        // Estadísticas para cards
        ViewBag.TotalAlcaldias = totalAlcaldias;
        ViewBag.AlcaldiasActivas = alcaldiasActivas;

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
        ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
        return View("Form", new AlcaldiaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlcaldiaViewModel model, IFormFile? LogoFile)
    {
        _logger.LogInformation($"Create POST - LogoFile: {LogoFile?.FileName}, Length: {LogoFile?.Length}");
        
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogWarning($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Procesar el archivo de logo si existe
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logos");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = $"{Guid.NewGuid()}_{LogoFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(fileStream);
                }
                
                model.Logo = $"/uploads/logos/{uniqueFileName}";
            }

            await _alcaldiaService.CreateAlcaldiaAsync(model);
            TempData["Success"] = "Alcaldía creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear alcaldía");
            ModelState.AddModelError("", ex.Message);
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var alcaldia = await _alcaldiaService.GetAlcaldiaByIdAsync(id);
        if (alcaldia == null)
        {
            return NotFound();
        }

        var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
        ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
        return View("Form", alcaldia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlcaldiaViewModel model, IFormFile? LogoFile)
    {
        _logger.LogInformation($"Edit POST - LogoFile: {LogoFile?.FileName}, Length: {LogoFile?.Length}");
        
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                _logger.LogWarning($"ModelState Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Obtener el logo actual antes de actualizar
            var alcaldiaActual = await _alcaldiaService.GetAlcaldiaByIdAsync(id);
            
            // Procesar el archivo de logo si existe uno nuevo
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logos");
                Directory.CreateDirectory(uploadsFolder);
                
                // Eliminar el logo anterior si existe
                if (!string.IsNullOrEmpty(alcaldiaActual?.Logo))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, alcaldiaActual.Logo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                
                var uniqueFileName = $"{Guid.NewGuid()}_{LogoFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(fileStream);
                }
                
                model.Logo = $"/uploads/logos/{uniqueFileName}";
            }
            else
            {
                // Mantener el logo actual si no se subió uno nuevo
                model.Logo = alcaldiaActual?.Logo;
            }

            await _alcaldiaService.UpdateAlcaldiaAsync(id, model);
            TempData["Success"] = "Alcaldía actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar alcaldía");
            ModelState.AddModelError("", ex.Message);
            var municipios = await _municipioService.GetAllMunicipiosAsync(incluirInactivos: false);
            ViewBag.Municipios = municipios.Select(m => new { Id = m.Id, Text = $"{m.Codigo} - {m.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var alcaldia = await _alcaldiaService.GetAlcaldiaByIdAsync(id);
            if (alcaldia == null)
            {
                return Json(new { success = false, message = "La alcaldía no fue encontrada." });
            }

            await _alcaldiaService.DeleteAlcaldiaAsync(id);
            return Json(new { success = true, message = $"La alcaldía '{alcaldia.Nit}' ha sido eliminada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar alcaldía");
            return Json(new { success = false, message = "Error al eliminar la alcaldía." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMunicipios(string searchTerm = "")
    {
        var municipios = string.IsNullOrEmpty(searchTerm)
            ? await _municipioService.GetAllMunicipiosAsync()
            : await _municipioService.SearchMunicipiosAsync(searchTerm);

        return Json(municipios.Select(m => new {
            id = m.Id,
            nombre = m.Nombre,
            departamento = m.NombreDepartamento
        }));
    }

    [HttpGet]
    public async Task<IActionResult> CheckNit(string nit, int? id)
    {
        var exists = await _alcaldiaService.NitExistsAsync(nit, id);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            // Solo devolver activas para dropdowns
            var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                alcaldias = alcaldias.Where(a => 
                    a.Nit.ToLower().Contains(term) || 
                    a.NombreMunicipio.ToLower().Contains(term));
            }

            var result = alcaldias
                .Take(limit)
                .Select(a => new {
                    id = a.Id,
                    nit = a.Nit,
                    municipio = a.NombreMunicipio
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar alcaldías");
            return Json(new List<object>());
        }
    }

    // GET: Alcaldias/GetNextId
    [HttpGet]
    public async Task<IActionResult> GetNextId()
    {
        try
        {
            var maxId = await _context.Alcaldias
                .Where(a => !a.IsDeleted)
                .Select(a => a.Id)
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
