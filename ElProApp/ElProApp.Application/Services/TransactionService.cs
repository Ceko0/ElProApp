namespace ElProApp.Application.Services
{
    using System;
    using System.Threading.Tasks;

    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Data;

    /// <summary>
    /// Executes operations inside a database transaction.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ElProAppDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TransactionService(ElProAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Executes an action inside a transaction.
        /// </summary>
        public async Task ExecuteAsync(Func<Task> action)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await action();

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Executes an action inside a transaction and returns a result.
        /// </summary>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await action();

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

