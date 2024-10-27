namespace ElProApp.Data.Repository
{
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;

    using Interfaces;

    /// <summary>
    /// Generic base repository class for performing CRUD operations.
    /// </summary>
    /// <typeparam name="TType">The type of entity the repository manages.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public class BaseRepository<TType, TId>(ElProAppDbContext context) : IRepository<TType, TId>
        where TType : class
    {
        private readonly ElProAppDbContext data = context;
        private readonly DbSet<TType> dbSet = context.Set<TType>();

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public TType GetById(TId id)
        {
            TType entity = this.dbSet.Find(id);
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public async Task<TType> GetByIdAsync(TId id)
        {
            TType entity = await this.dbSet.FindAsync(id);
            return entity;
        }

        /// <summary>
        /// Finds the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <returns>The first matching entity, or null if no match is found.</returns>
        public TType FirstOrDefault(Func<TType, bool> predicate)
        {
            TType entity = this.dbSet.FirstOrDefault(predicate);
            return entity;
        }

        /// <summary>
        /// Asynchronously finds the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <returns>The first matching entity, or null if no match is found.</returns>
        public async Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            TType entity = await this.dbSet.FirstOrDefaultAsync(predicate);
            return entity;
        }

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <returns>An enumerable collection of all entities.</returns>
        public IEnumerable<TType> GetAll() => this.dbSet.AsQueryable();

        /// <summary>
        /// Asynchronously retrieves all entities.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a collection of all entities.</returns>
        public async Task<IEnumerable<TType>> GetAllAsync() => await this.dbSet.ToArrayAsync();

        /// <summary>
        /// Retrieves all attached entities (e.g., tracked by the context).
        /// </summary>
        /// <returns>An IQueryable collection of all attached entities.</returns>
        public IQueryable<TType> GetAllAttached() => this.dbSet.AsQueryable();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public void Add(TType item)
        {
            this.dbSet.Add(item);
            this.data.SaveChanges();
        }

        /// <summary>
        /// Asynchronously adds a new entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
            await this.data.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a range of entities to the repository.
        /// </summary>
        /// <param name="items">An array of entities to add.</param>
        public void AddRange(TType[] items)
        {
            this.dbSet.AddRange(items);
            this.data.SaveChanges();
        }

        /// <summary>
        /// Asynchronously adds a range of entities to the repository.
        /// </summary>
        /// <param name="items">An array of entities to add.</param>
        public async Task AddRangeAsync(TType[] items)
        {
            await this.dbSet.AddRangeAsync(items);
            await this.data.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        public bool Delete(TId id)
        {
            TType entity = this.GetById(id);
            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);
            this.data.SaveChanges();
            return true;
        }

        /// <summary>
        /// Asynchronously deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>True if the entity was successfully deleted; otherwise, false.</returns>
        public async Task<bool> DeleteAsync(TId id)
        {
            TType entity = await this.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);
            await this.data.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Soft deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        /// <returns>True if the entity was successfully soft deleted; otherwise, false.</returns>
        public bool SoftDeletе(TId id)
        {
            var entity = dbSet.Find(id);

            if (entity == null)
            {
                return false;
            }

            var isDeletedProperty = typeof(TType).GetProperty("IsDeleted");

            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
            {
                isDeletedProperty.SetValue(entity, true);
                data.SaveChanges();
                return true;
            }
            else
            {
                throw new InvalidOperationException($"The entity {typeof(TType).Name} does not have an IsDeleted property.");
            }
        }

        /// <summary>
        /// Asynchronously soft deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        /// <returns>True if the entity was successfully soft deleted; otherwise, false.</returns>
        public async Task<bool> SoftDeleteAsync(TId id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            var isDeletedProperty = typeof(TType).GetProperty("IsDeleted");

            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
            {
                isDeletedProperty.SetValue(entity, true);
                await data.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new InvalidOperationException($"The entity {typeof(TType).Name} does not have an IsDeleted property.");
            }
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>True if the entity was successfully updated; otherwise, false.</returns>
        public bool Update(TType item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.data.Entry(item).State = EntityState.Modified;
                this.data.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously updates an existing entity.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>True if the entity was successfully updated; otherwise, false.</returns>
        public async Task<bool> UpdateAsync(TType item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.data.Entry(item).State = EntityState.Modified;
                await this.data.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Saves changes to the repository.
        /// </summary>
        /// <returns>True if the changes were successfully saved; otherwise, false.</returns>
        public bool Save()
        {
            try
            {
                this.data.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously saves changes to the repository.
        /// </summary>
        /// <returns>True if the changes were successfully saved; otherwise, false.</returns>
        public async Task<bool> SaveAsync()
        {
            try
            {
                await this.data.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
