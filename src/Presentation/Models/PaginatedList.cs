using Microsoft.EntityFrameworkCore;

namespace Proyecto_alcaldia.Presentation.Models;

/// <summary>
/// Clase genérica para manejar listas paginadas con soporte para consultas asíncronas a la base de datos.
/// Implementa paginación del lado del servidor para evitar cargar todos los datos en memoria.
/// </summary>
/// <typeparam name="T">Tipo de los elementos de la lista</typeparam>
public class PaginatedList<T> : List<T>
{
    /// <summary>
    /// Página actual (1-indexed)
    /// </summary>
    public int PageIndex { get; private set; }
    
    /// <summary>
    /// Número total de páginas
    /// </summary>
    public int TotalPages { get; private set; }
    
    /// <summary>
    /// Número total de elementos
    /// </summary>
    public int TotalCount { get; private set; }
    
    /// <summary>
    /// Tamaño de página (elementos por página)
    /// </summary>
    public int PageSize { get; private set; }
    
    /// <summary>
    /// Tamaños de página disponibles para selección
    /// </summary>
    public static readonly int[] AvailablePageSizes = { 5, 10, 20, 50, 100 };

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        PageSize = pageSize;

        AddRange(items);
    }

    /// <summary>
    /// Indica si hay una página anterior disponible
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Indica si hay una página siguiente disponible
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Índice del primer elemento en la página actual (1-indexed)
    /// </summary>
    public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;

    /// <summary>
    /// Índice del último elemento en la página actual
    /// </summary>
    public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalCount);

    /// <summary>
    /// Crea una lista paginada a partir de una consulta IQueryable asíncrona.
    /// Solo trae de la base de datos los elementos de la página solicitada.
    /// </summary>
    /// <param name="source">Consulta IQueryable</param>
    /// <param name="pageIndex">Índice de página (1-indexed)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <returns>Lista paginada</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        // Asegurar valores mínimos válidos
        pageIndex = Math.Max(1, pageIndex);
        pageSize = Math.Max(1, pageSize);

        // Contar total de elementos (una sola consulta COUNT)
        var count = await source.CountAsync();
        
        // Si la página solicitada es mayor al total de páginas, ajustar
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        if (totalPages > 0 && pageIndex > totalPages)
        {
            pageIndex = totalPages;
        }

        // Traer solo los elementos de la página actual (Skip + Take en la BD)
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    /// Crea una lista paginada a partir de una lista en memoria.
    /// Útil cuando ya se tiene la lista completa.
    /// </summary>
    /// <param name="source">Lista fuente</param>
    /// <param name="pageIndex">Índice de página (1-indexed)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <returns>Lista paginada</returns>
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        // Asegurar valores mínimos válidos
        pageIndex = Math.Max(1, pageIndex);
        pageSize = Math.Max(1, pageSize);

        var list = source.ToList();
        var count = list.Count;
        
        // Si la página solicitada es mayor al total de páginas, ajustar
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        if (totalPages > 0 && pageIndex > totalPages)
        {
            pageIndex = totalPages;
        }

        var items = list
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    /// Obtiene el rango de páginas a mostrar en la navegación
    /// </summary>
    /// <param name="maxPagesToShow">Máximo número de páginas a mostrar</param>
    /// <returns>Lista de números de página a mostrar</returns>
    public IEnumerable<int> GetPageRange(int maxPagesToShow = 5)
    {
        var halfRange = maxPagesToShow / 2;
        var startPage = Math.Max(1, PageIndex - halfRange);
        var endPage = Math.Min(TotalPages, startPage + maxPagesToShow - 1);
        
        // Ajustar inicio si estamos cerca del final
        startPage = Math.Max(1, endPage - maxPagesToShow + 1);
        
        return Enumerable.Range(startPage, endPage - startPage + 1);
    }
}
