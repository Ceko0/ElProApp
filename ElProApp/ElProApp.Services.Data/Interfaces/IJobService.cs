namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using Web.Models.Job;

    public interface IJobService
    {
        /// <summary>
        /// Retrieves all JobsList as a queryable collection, including related entities for advanced querying.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="Job"/> entities.
        /// </returns>
        IQueryable<Job> GetAllAttached();

        /// <summary>
        /// Adds a new job based on the provided input model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="JobInputModel"/> containing job details.
        /// </param>
        /// <returns>
        /// The unique ID of the newly added job.
        /// </returns>
        Task<string> AddAsync(JobInputModel model);

        /// <summary>
        /// Retrieves a job by its unique ID.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the job.
        /// </param>
        /// <returns>
        /// A <see cref="JobViewModel"/> representing the requested job, or null if not found.
        /// </returns>
        Task<JobViewModel> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves the edit model for a specific job by its unique ID.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the job.
        /// </param>
        /// <returns>
        /// A <see cref="JobEditInputModel"/> containing editable details of the job.
        /// </returns>
        Task<JobEditInputModel> EditByIdAsync(string id);

        /// <summary>
        /// Updates an existing job using the provided edit input model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="JobEditInputModel"/> containing updated job details.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the update operation was successful.
        /// </returns>
        Task<bool> EditByModelAsync(JobEditInputModel model);

        /// <summary>
        /// Retrieves all JobsList as a list for simpler operations or UI display.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="JobViewModel"/> representing all JobsList.
        /// </returns>
        Task<ICollection<JobViewModel>> GetAllAsync();

        /// <summary>
        /// Soft deletes a job by marking it as inactive instead of removing it permanently.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the job.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the soft delete operation was successful.
        /// </returns>
        Task<bool> SoftDeleteAsync(string id);
    }
}
