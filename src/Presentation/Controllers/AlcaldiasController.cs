using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

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
    public async Task<IActionResult> Index()
    {
        var alcaldias = await _alcaldiaService.GetAllAlcaldiasAsync(incluirInactivas: true);
        return View(alcaldias);
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
}
