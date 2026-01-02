using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Infrastructure.Data;
using Proyecto_alcaldia.Application.Services;
using System.Security.Claims;

namespace Proyecto_alcaldia.Presentation.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public BaseController(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Cargar el tema y foto del usuario antes de ejecutar la acción
        await CargarDatosUsuario();
        await base.OnActionExecutionAsync(context, next);
    }

    protected async Task CargarDatosUsuario()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrEmpty(userEmail))
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == userEmail);
            
            ViewBag.TemaUsuario = usuario?.TemaColor ?? "default";
            ViewBag.NombreUsuario = usuario?.NombreCompleto ?? "Usuario";
            
            // Cargar y desencriptar foto de perfil si existe
            if (usuario != null && !string.IsNullOrEmpty(usuario.FotoPerfilEncriptada))
            {
                try
                {
                    var imageEncryptionService = _serviceProvider.GetService<IImageEncryptionService>();
                    if (imageEncryptionService != null)
                    {
                        var fotoBytes = imageEncryptionService.DecryptImage(usuario.FotoPerfilEncriptada);
                        ViewBag.FotoPerfilBase64 = Convert.ToBase64String(fotoBytes);
                    }
                }
                catch
                {
                    ViewBag.FotoPerfilBase64 = null;
                }
            }
            else
            {
                ViewBag.FotoPerfilBase64 = null;
            }
        }
        else
        {
            ViewBag.TemaUsuario = "default";
            ViewBag.NombreUsuario = "Usuario";
            ViewBag.FotoPerfilBase64 = null;
        }
    }
}
