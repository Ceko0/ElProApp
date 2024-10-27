
namespace ElProApp.Data.Repository.Interfaces
{
    using System.Linq.Expressions;

    /// <summary>
    /// Generic repository interface for basic CRUD operations.
    /// </summary>
    /// <typeparam name="TType">The type of entity the repository manages.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public interface IRepository<TType, TId>
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        TType GetById(TId id);

        /// <summary>
        /// Asynchronously gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<TType> GetByIdAsync(TId id);

        /// <summary>
        /// Finds the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <returns>The first matching entity, or default if no match is found.</returns>
        TType FirstOrDefault(Func<TType, bool> predicate);

        /// <summary>
        /// Asynchronously finds the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <returns>The first matching entity, or default if no match is found.</returns>
        Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>An enumerable collection of all entities.</returns>
        IEnumerable<TType> GetAll();

        /// <summary>
        /// Asynchronously gets all entities.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a collection of all entities.</returns>
        Task<IEnumerable<TType>> GetAllAsync();

        /// <summary>
        /// Gets all attached entities (e.g., tracked by the context).
        /// </summary>
        /// <returns>An IQueryable collection of all attached entities.</returns>
        IQueryable<TType> GetAllAttached();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        void Add(TType item);

        /// <summary>
        /// Asynchronously adds a new entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        Task AddAsync(TType item);

        /// <summary>
        /// Adds a range of entities to the repository.
        /// </summary>
        /// <param name="items">An array of entities to add.</param>
        void AddRange(TType[] items);

        /// <summary>
        /// Asynchronously adds a range of entities to the repository.
        /// </summary>
        /// <param name="items">An array of entities to add.</param>
        Task AddRangeAsync(TType[] items);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        bool Delete(TId id);

        /// <summary>
        /// Asynchronously deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(TId id);

        /// <summary>
        /// Soft deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        /// <returns>True if the entity was successfully soft deleted; otherwise, false.</returns>
        bool SoftDeletе(TId id);

        /// <summary>
        /// Asynchronously soft deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        /// <returns>True if the entity was successfully soft deleted; otherwise, false.</returns>
        Task<bool> SoftDeleteAsync(TId id);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>True if the entity was successfully updated; otherwise, false.</returns>
        bool Update(TType item);

        /// <summary>
        /// Asynchronously updates an existing entity.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>True if the entity was successfully updated; otherwise, false.</returns>
        Task<bool> UpdateAsync(TType item);

        /// <summary>
        /// Saves changes to the repository.
        /// </summary>
        /// <returns>True if the changes were successfully saved; otherwise, false.</returns>
        bool Save();

        /// <summary>
        /// Asynchronously saves changes to the repository.
        /// </summary>
        /// <returns>True if the changes were successfully saved; otherwise, false.</returns>
        Task<bool> SaveAsync();
    }
}
