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

    /// <summary>
    /// Obtiene el ID del usuario actual autenticado
    /// </summary>
    /// <returns>ID del usuario como string, o "Sistema" si no está autenticado</returns>
    protected async Task<string> ObtenerUsuarioIdActual()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrEmpty(userEmail))
        {
            var usuario = await _context.Usuarios
                .Where(u => u.CorreoElectronico == userEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
            
            return usuario > 0 ? usuario.ToString() : "Sistema";
        }
        return "Sistema";
    }

    /// <summary>
    /// Obtiene el próximo código consecutivo para una tabla
    /// </summary>
    /// <param name="tableName">Nombre de la tabla</param>
    /// <returns>El próximo ID disponible</returns>
    protected async Task<int> GetNextCodigoAsync(string tableName)
    {
        var maxId = 0;
        switch (tableName.ToLower())
        {
            case "usuarios":
                maxId = await _context.Usuarios.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            case "roles":
                maxId = await _context.Roles.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            case "alcaldias":
                maxId = await _context.Alcaldias.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            case "secretarias":
                maxId = await _context.Secretarias.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            case "subsecretarias":
                maxId = await _context.Subsecretarias.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            case "responsables":
                maxId = await _context.Responsables.MaxAsync(x => (int?)x.Id) ?? 0;
                break;
            default:
                throw new ArgumentException($"Tabla no soportada: {tableName}");
        }
        return maxId + 1;
    }

    /// <summary>
    /// Maneja errores de base de datos y retorna mensajes amigables
    /// </summary>
    /// <param name="ex">Excepción capturada</param>
    /// <returns>Mensaje de error amigable para el usuario</returns>
    protected string ObtenerMensajeErrorBaseDatos(Exception ex)
    {
        if (ex is DbUpdateException dbEx)
        {
            var innerException = dbEx.InnerException?.Message ?? string.Empty;
            
            // Detectar errores comunes de PostgreSQL
            if (innerException.Contains("duplicate key") || innerException.Contains("23505"))
            {
                if (innerException.Contains("codigo"))
                    return "Ya existe un registro con ese código. Por favor, use un código diferente";
                if (innerException.Contains("nombre"))
                    return "Ya existe un registro con ese nombre. Por favor, use un nombre diferente";
                if (innerException.Contains("correo") || innerException.Contains("email"))
                    return "Ya existe un registro con ese correo electrónico";
                
                return "Ya existe un registro con esos datos. Por favor, verifique la información ingresada";
            }
            
            if (innerException.Contains("foreign key") || innerException.Contains("23503"))
            {
                return "No se puede completar la operación porque existen registros relacionados";
            }
            
            if (innerException.Contains("violates check constraint") || innerException.Contains("23514"))
            {
                return "Los datos ingresados no cumplen con las restricciones de validación";
            }
            
            if (innerException.Contains("violates not-null constraint") || innerException.Contains("23502"))
            {
                return "Faltan campos requeridos. Por favor, complete todos los campos obligatorios";
            }
        }
        
        return "Ocurrió un error al procesar la solicitud. Por favor, intente nuevamente";
    }

    /// <summary>
    /// Configura el ViewBag con los datos de paginación para la vista
    /// </summary>
    /// <param name="pageIndex">Página actual (1-indexed)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="totalCount">Total de elementos</param>
    protected void ConfigurarPaginacion(int pageIndex, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        ViewBag.PageIndex = pageIndex;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.FirstItemIndex = totalCount > 0 ? (pageIndex - 1) * pageSize + 1 : 0;
        ViewBag.LastItemIndex = Math.Min(pageIndex * pageSize, totalCount);
        ViewBag.HasPreviousPage = pageIndex > 1;
        ViewBag.HasNextPage = pageIndex < totalPages;
    }

    /// <summary>
    /// Valida y normaliza los parámetros de paginación
    /// </summary>
    /// <param name="page">Página solicitada</param>
    /// <param name="pageSize">Tamaño de página solicitado</param>
    /// <returns>Tupla con página y tamaño validados</returns>
    protected (int page, int pageSize) ValidarParametrosPaginacion(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = new[] { 5, 10, 20, 50, 100 }.Contains(pageSize) ? pageSize : 5;
        return (page, pageSize);
    }
}
