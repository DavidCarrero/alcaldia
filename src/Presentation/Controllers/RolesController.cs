using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class RolesController : BaseController
{
    private readonly IRolService _rolService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRolService rolService, 
        ILogger<RolesController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _rolService = rolService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var roles = await _rolService.GetAllRolesAsync(incluirInactivos: true);
        var estadisticas = await _rolService.GetEstadisticasAsync();

        ViewBag.TotalRoles = estadisticas.totalRoles;
        ViewBag.PermisosConfigurados = estadisticas.permisosConfigurados;
        ViewBag.UsuariosAsignados = estadisticas.usuariosAsignados;
        ViewBag.RolesPersonalizados = estadisticas.rolesPersonalizados;

        return View(roles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new RolViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            await _rolService.CreateRolAsync(model);
            TempData["Success"] = "Rol creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            ModelState.AddModelError("", ex.Message);
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var rol = await _rolService.GetRolByIdAsync(id);
        if (rol == null)
        {
            return NotFound();
        }

        return View("Form", rol);
    }

    [HttpPatch]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RolViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        try
        {
            await _rolService.UpdateRolAsync(id, model);
            TempData["Success"] = "Rol actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol");
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
            var rol = await _rolService.GetRolByIdAsync(id);
            if (rol == null)
            {
                return Json(new { success = false, message = "El rol no fue encontrado." });
            }

            await _rolService.DeleteRolAsync(id);
            return Json(new { success = true, message = $"El rol '{rol.Nombre}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar rol");
            return Json(new { success = false, message = "Error al eliminar el rol." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var roles = await _rolService.SearchRolesAsync(term);
        return Json(roles);
    }

    [HttpGet]
    public async Task<IActionResult> CheckName(string nombre, int? id)
    {
        var exists = await _rolService.NameExistsAsync(nombre, id);
        return Json(!exists);
    }
}
