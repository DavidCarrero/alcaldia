using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_alcaldia.Application.Services;
using Proyecto_alcaldia.Application.ViewModels;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Presentation.Controllers;

[Authorize]
public class ProductosController : BaseController
{
    private readonly IProductoService _productoService;
    private readonly IAlcaldiaService _alcaldiaService;
    private readonly IProgramaService _programaService;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(
        IProductoService productoService,
        IAlcaldiaService alcaldiaService,
        IProgramaService programaService,
        ILogger<ProductosController> logger,
        ApplicationDbContext context,
        IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
        _productoService = productoService;
        _alcaldiaService = alcaldiaService;
        _programaService = programaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.GetAllProductosAsync(incluirInactivas: true);
        return View(productos);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
        ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        var model = new ProductoViewModel
        {
            AlcaldiaId = AlcaldiaIdUsuarioActual ?? 0
        };
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
            ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }

        try
        {
            // Validar que el usuario tenga una alcaldía asignada
            if (!ValidarAlcaldiaId())
            {
                var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
                ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
                return View("Form", model);
            }

            // Asignar alcaldía del usuario logueado
            model.AlcaldiaId = ObtenerAlcaldiaId();
            
            // Normalizar FK opcionales (convertir 0 a null)
            model.ProgramaId = NormalizarIdOpcional(model.ProgramaId);
            
            await _productoService.CreateProductoAsync(model);
            TempData["Success"] = "Producto creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            ModelState.AddModelError("", ex.Message);
            var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
            ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _productoService.GetProductoByIdAsync(id);
        if (producto == null)
        {
            return NotFound();
        }

        var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
        ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });

        return View("Form", producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
            ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }

        try
        {
            await _productoService.UpdateProductoAsync(id, model);
            TempData["Success"] = "Producto actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto");
            ModelState.AddModelError("", ex.Message);
            var programas = await _programaService.GetAllProgramasAsync(incluirInactivas: false);
            ViewBag.Programas = programas.Select(p => new { Id = p.Id, Text = $"{p.Codigo} - {p.Nombre}" });
            return View("Form", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return Json(new { success = false, message = "El producto no fue encontrado." });
            }

            await _productoService.DeleteProductoAsync(id);
            return Json(new { success = true, message = $"El producto '{producto.Codigo}' ha sido eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar producto");
            return Json(new { success = false, message = "Error al eliminar el producto." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "", int limit = 20)
    {
        try
        {
            var productos = await _productoService.GetAllProductosAsync(incluirInactivas: false);
            
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                productos = productos.Where(p => 
                    p.Codigo.ToLower().Contains(term) || 
                    p.Nombre.ToLower().Contains(term));
            }

            var result = productos
                .Take(limit)
                .Select(p => new {
                    id = p.Id,
                    codigo = p.Codigo,
                    nombre = p.Nombre
                });

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos");
            return Json(new List<object>());
        }
    }
}
