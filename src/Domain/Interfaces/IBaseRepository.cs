using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Domain.Interfaces;

/// <summary>
/// Interfaz base para todos los repositorios del sistema
/// Proporciona operaciones CRUD estándar
/// </summary>
/// <typeparam name="T">Entidad que hereda de BaseEntity</typeparam>
public interface IBaseRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtiene todos los registros
    /// </summary>
    /// <param name="incluirInactivos">Si es true, incluye registros inactivos</param>
    /// <returns>Lista de entidades</returns>
    Task<IEnumerable<T>> GetAllAsync(bool incluirInactivos = false);

    /// <summary>
    /// Obtiene un registro por su ID
    /// </summary>
    /// <param name="id">ID del registro</param>
    /// <returns>Entidad encontrada o null</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo registro
    /// </summary>
    /// <param name="entity">Entidad a crear</param>
    /// <returns>Entidad creada con ID asignado</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Actualiza un registro existente
    /// </summary>
    /// <param name="entity">Entidad con datos actualizados</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Elimina (desactiva) un registro de forma lógica
    /// </summary>
    /// <param name="id">ID del registro a eliminar</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe un registro con el ID especificado
    /// </summary>
    /// <param name="id">ID a verificar</param>
    /// <returns>True si existe, false en caso contrario</returns>
    Task<bool> ExistsAsync(int id);
}
