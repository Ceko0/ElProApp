namespace ElProApp.Application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides operations for applying and rolling back material consumption.
    /// </summary>
    public interface IMaterialConsumptionService
    {
        /// <summary>
        /// Applies material consumption with price snapshot.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materials">
        /// A dictionary where the key is the material identifier and the value contains quantity and price.
        /// </param>
        Task ApplyAsync(Guid jobDoneId, Guid buildingId, Dictionary<Guid, (decimal Quantity, decimal Price)> materials);

        /// <summary>
        /// Rolls back material consumption.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        Task RollbackAsync(Guid jobDoneId);
    }
}