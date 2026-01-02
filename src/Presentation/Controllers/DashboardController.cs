using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // Obtener estadísticas para el dashboard
        var totalUsuarios = await _context.Usuarios.CountAsync(u => u.Activo);
        var totalAlcaldias = await _context.Alcaldias.CountAsync(a => a.Activo);
        var totalProyectos = await _context.Proyectos.CountAsync(p => p.Activo);
        var totalActividades = await _context.Actividades.CountAsync(a => a.Activo);

        ViewBag.TotalUsuarios = totalUsuarios;
        ViewBag.TotalAlcaldias = totalAlcaldias;
        ViewBag.TotalProyectos = totalProyectos;
        ViewBag.TotalActividades = totalActividades;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarTema([FromBody] TemaRequest request)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == userEmail);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.TemaColor = request.Tema;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Tema guardado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el tema del usuario");
            return StatusCode(500, new { success = false, message = "Error al guardar el tema" });
        }
    }
}

public class TemaRequest
{
    public string Tema { get; set; } = "default";
}
