namespace Proyecto_alcaldia.Application.Services;

/// <summary>
/// Interfaz base para todos los servicios del sistema
/// Proporciona operaciones CRUD estándar con ViewModels
/// </summary>
/// <typeparam name="TViewModel">ViewModel asociado al servicio</typeparam>
public interface IBaseService<TViewModel> where TViewModel : class
{
    /// <summary>
    /// Obtiene todos los registros como ViewModels
    /// </summary>
    /// <param name="incluirInactivos">Si es true, incluye registros inactivos</param>
    /// <returns>Lista de ViewModels</returns>
    Task<IEnumerable<TViewModel>> GetAllAsync(bool incluirInactivos = false);

    /// <summary>
    /// Obtiene un registro por su ID como ViewModel
    /// </summary>
    /// <param name="id">ID del registro</param>
    /// <returns>ViewModel encontrado o null</returns>
    Task<TViewModel?> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo registro desde un ViewModel
    /// </summary>
    /// <param name="viewModel">ViewModel con los datos</param>
    /// <returns>ViewModel del registro creado</returns>
    Task<TViewModel> CreateAsync(TViewModel viewModel);

    /// <summary>
    /// Actualiza un registro existente desde un ViewModel
    /// </summary>
    /// <param name="viewModel">ViewModel con datos actualizados</param>
    Task UpdateAsync(TViewModel viewModel);

    /// <summary>
    /// Elimina (desactiva) un registro de forma lógica
    /// </summary>
    /// <param name="id">ID del registro a eliminar</param>
    Task DeleteAsync(int id);
}
