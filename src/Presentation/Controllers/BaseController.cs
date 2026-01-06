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
    protected int? AlcaldiaIdUsuarioActual { get; private set; }

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
            AlcaldiaIdUsuarioActual = usuario?.AlcaldiaId;
            
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
            AlcaldiaIdUsuarioActual = null;
        }
    }

    /// <summary>
    /// Valida si el usuario tiene un AlcaldiaId válido asignado
    /// </summary>
    /// <returns>True si el AlcaldiaId es válido, False si no</returns>
    protected bool ValidarAlcaldiaId()
    {
        if (!AlcaldiaIdUsuarioActual.HasValue || AlcaldiaIdUsuarioActual.Value == 0)
        {
            ModelState.AddModelError("", "No se pudo obtener la alcaldía del usuario. Por favor, contacte al administrador para que le asigne una alcaldía.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Obtiene el AlcaldiaId del usuario actual o 0 si no tiene asignado
    /// </summary>
    /// <returns>AlcaldiaId del usuario o 0</returns>
    protected int ObtenerAlcaldiaId()
    {
        return AlcaldiaIdUsuarioActual ?? 0;
    }

    /// <summary>
    /// Convierte un ID con valor 0 a null para FK opcionales
    /// </summary>
    /// <param name="id">El ID a validar</param>
    /// <returns>El ID si es mayor a 0, o null si es 0 o menor</returns>
    protected int? NormalizarIdOpcional(int? id)
    {
        return id.HasValue && id.Value > 0 ? id.Value : null;
    }
}
