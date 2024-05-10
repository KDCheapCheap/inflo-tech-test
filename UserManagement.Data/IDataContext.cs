using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Data;

public interface IDataContext
{
    /// <summary>
    /// Get a list of items
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;
    Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class;

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    TEntity Create<TEntity>(TEntity entity) where TEntity : class;
    Task<TEntity> CreateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Get an entity by a <see cref="long"/> Id.
    /// </summary>
    TEntity GetById<TEntity>(long id) where TEntity : class;
    Task<TEntity> GetByIdAsync<TEntity>(long id) where TEntity : class;

    /// <summary>
    /// Gets an Entity by ID without tracking the entity.
    /// </summary>
    TEntity GetByIdUntracked<TEntity>(long id) where TEntity : class;
    Task<TEntity> GetByIdUntrackedAsync<TEntity>(long id) where TEntity : class;

    /// <summary>
    /// Update an existing item matching the ID
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    TEntity Update<TEntity>(TEntity entity) where TEntity : class;
    Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Deletes an Entity from the database.
    /// </summary>
    void Delete<TEntity>(TEntity entity) where TEntity : class;
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
}
