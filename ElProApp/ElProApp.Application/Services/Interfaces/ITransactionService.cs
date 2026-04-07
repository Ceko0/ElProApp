namespace ElProApp.Application.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a mechanism to execute operations within a database transaction.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Executes an asynchronous action inside a transaction.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        Task ExecuteAsync(Func<Task> action);

        /// <summary>
        /// Executes an asynchronous action inside a transaction and returns a result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the action.</returns>
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }

}
