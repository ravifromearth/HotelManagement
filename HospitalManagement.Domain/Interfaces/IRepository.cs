namespace HospitalManagement.Domain.Interfaces;

/// <summary>
/// Generic repository interface
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by id
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>Entity</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Add entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    void Update(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(T entity);
}