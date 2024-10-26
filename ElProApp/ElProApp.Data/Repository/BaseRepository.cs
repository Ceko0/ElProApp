namespace ElProApp.Data.Repository
{
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;

    using Interfaces;
    using System.Security.Cryptography;

    public class BaseRepository<TType, TId>(ElProAppDbContext context) : IRepository<TType, TId>
        where TType : class
    {
        private readonly ElProAppDbContext data = context;
        private readonly DbSet<TType> dbSet = context.Set<TType>();

        public TType GetById(TId id)
        {
            TType entity = this.dbSet
                .Find(id);
            return entity;
        }

        public async Task<TType> GetByIdAsync(TId id)
        {
            TType entity = await this.dbSet
                .FindAsync(id);

            return entity;
        }

        public TType FirstOrDefault(Func<TType, bool> predicate)
        {
            TType entity = this.dbSet
                .FirstOrDefault(predicate);

            return entity;
        }

        public async Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            TType entity = await this.dbSet
                .FirstOrDefaultAsync(predicate);

            return entity;
        }

        public IEnumerable<TType> GetAll()
        {
            return this.dbSet.ToArray();
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await this.dbSet.ToArrayAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return this.dbSet.AsQueryable();
        }

        public void Add(TType item)
        {
            this.dbSet.Add(item);
            this.data.SaveChanges();
        }

        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
            await this.data.SaveChangesAsync();
        }

        public void AddRange(TType[] items)
        {
            this.dbSet.AddRange(items);
            this.data.SaveChanges();
        }

        public async Task AddRangeAsync(TType[] items)
        {
            await this.dbSet.AddRangeAsync(items);
            await this.data.SaveChangesAsync();
        }

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
    }
}
