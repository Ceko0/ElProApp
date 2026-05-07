namespace ElProApp.Application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Provides operations for managing mappings between JobDone and Material entities.
    /// Handles creation, retrieval, update and deletion of mappings,
    /// including quantity tracking and snapshot pricing.
    /// </summary>
    public interface IJobDoneMaterialMappingService
    {
        /// <summary>
        /// Creates a new job-done material mapping.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier (string GUID).</param>
        /// <param name="materialId">The material identifier (string GUID).</param>
        /// <param name="quantity">The quantity used.</param>
        /// <param name="unitPrice">The unit price at the time (snapshot).</param>
        /// <returns>The created mapping entity.</returns>
        Task<JobDoneMaterialMapping> AddAsync(
            string jobDoneId,
            string materialId,
            decimal quantity,
            decimal unitPrice);

        /// <summary>
        /// Updates the quantity of an existing mapping.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The new quantity.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateQuantityAsync(Guid jobDoneId, Guid materialId, decimal quantity);

        /// <summary>
        /// Removes a mapping by composite key (JobDoneId + MaterialId).
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByIdsAsync(Guid jobDoneId, Guid materialId);

        /// <summary>
        /// Retrieves all material mappings for a specific job-done identifier.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <returns>A collection of mappings.</returns>
        Task<ICollection<JobDoneMaterialMapping>> GetByJobDoneIdAsync(string jobDoneId);

        /// <summary>
        /// Retrieves all job-done mappings for a specific material identifier.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <returns>A collection of mappings.</returns>
        Task<ICollection<JobDoneMaterialMapping>> GetByMaterialIdAsync(string materialId);

        /// <summary>
        /// Retrieves all mappings including related entities.
        /// </summary>
        /// <returns>A collection of mappings.</returns>
        Task<ICollection<JobDoneMaterialMapping>> GetAllAttachedAsync();

        /// <summary>
        /// Returns a queryable collection of mappings.
        /// </summary>
        /// <returns>An <see cref="IQueryable{JobDoneMaterialMapping}"/>.</returns>
        IQueryable<JobDoneMaterialMapping> GetAllAttached();

        /// <summary>
        /// Checks whether a mapping exists for given identifiers.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <returns>True if mapping exists; otherwise, false.</returns>
        bool Any(string jobDoneId, string materialId);

        /// <summary>
        /// Removes a mapping using the provided entity instance.
        /// </summary>
        /// <param name="mapping">The mapping to remove.</param>
        /// <returns>True if successfully removed.</returns>
        Task<bool> RemoveAsync(JobDoneMaterialMapping mapping);

        /// <summary>
        /// Soft deletes all mappings for a specific job-done record.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveByJobDoneIdAsync(string jobDoneId);
    }
}