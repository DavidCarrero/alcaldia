using Microsoft.EntityFrameworkCore;

namespace Proyecto_alcaldia.Application.Common;

/// <summary>
/// Clase genérica para manejar listas paginadas desde la base de datos.
/// </summary>
/// <typeparam name="T">Tipo de elemento en la lista</typeparam>
public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public int PageSize { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalCount = count;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

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
    /// Obtiene el índice del primer elemento mostrado
    /// </summary>
    public int FirstItemIndex => TotalCount == 0 ? 0 : (PageIndex - 1) * PageSize + 1;

    /// <summary>
    /// Obtiene el índice del último elemento mostrado
    /// </summary>
    public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalCount);

    /// <summary>
    /// Crea una lista paginada desde un IQueryable de forma asíncrona
    /// </summary>
    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    /// Crea una lista paginada desde una lista en memoria
    /// </summary>
    public static PaginatedList<T> Create(
        IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var list = source.ToList();
        var count = list.Count;
        var items = list
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    /// Obtiene los números de página a mostrar en la paginación
    /// </summary>
    public IEnumerable<int> GetPageNumbers(int maxPages = 5)
    {
        int startPage = Math.Max(1, PageIndex - maxPages / 2);
        int endPage = Math.Min(TotalPages, startPage + maxPages - 1);
        
        if (endPage - startPage + 1 < maxPages)
        {
            startPage = Math.Max(1, endPage - maxPages + 1);
        }

        return Enumerable.Range(startPage, endPage - startPage + 1);
    }
}
