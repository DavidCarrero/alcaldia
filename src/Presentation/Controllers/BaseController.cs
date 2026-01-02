using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Infrastructure.Data;
using System.Security.Claims;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;

    public BaseController(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Cargar el tema del usuario antes de ejecutar la acción
        await CargarTemaUsuario();
        await base.OnActionExecutionAsync(context, next);
    }

    protected async Task CargarTemaUsuario()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrEmpty(userEmail))
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == userEmail);
            
            ViewBag.TemaUsuario = usuario?.TemaColor ?? "default";
        }
        else
        {
            ViewBag.TemaUsuario = "default";
        }
    }
}
