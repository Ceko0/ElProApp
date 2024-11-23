namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.JobDone;

    public interface IJobDoneService
    {
        /// <summary>
        /// Adds a new "Job Done" entry with default values.
        /// </summary>
        /// <returns>
        /// A <see cref="JobDoneInputModel"/> object initialized with default values for creating a new "Job Done" entry.
        /// </returns>
        Task<JobDoneInputModel> AddAsync();

        /// <summary>
        /// Adds a new "Job Done" entry based on the provided input model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="JobDoneInputModel"/> containing the details of the job done.
        /// </param>
        /// <returns>
        /// A string representing the unique ID of the newly created "Job Done" entry.
        /// </returns>
        Task<string> AddAsync(JobDoneInputModel model);

        /// <summary>
        /// Retrieves a "Job Done" entry by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the "Job Done" entry.</param>
        /// <returns>
        /// A <see cref="JobDoneViewModel"/> containing the details of the requested "Job Done" entry.
        /// </returns>
        Task<JobDoneViewModel> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves the edit input model for a specific "Job Done" entry by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the "Job Done" entry.</param>
        /// <returns>
        /// A <see cref="JobDoneEditInputModel"/> for editing the specified "Job Done" entry.
        /// </returns>
        Task<JobDoneEditInputModel> EditByIdAsync(string id);

        /// <summary>
        /// Updates an existing "Job Done" entry using the provided edit model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="JobDoneEditInputModel"/> containing the updated details of the job done.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the update operation was successful.
        /// </returns>
        Task<bool> EditByModelAsync(JobDoneEditInputModel model);

        /// <summary>
        /// Retrieves all "Job Done" entries as a collection of view models.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="JobDoneViewModel"/> objects representing all "Job Done" entries.
        /// </returns>
        Task<ICollection<JobDoneViewModel>> GetAllAsync();

        /// <summary>
        /// Retrieves all "Job Done" entries as a queryable collection with attached related entities.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="JobDone"/> entities for advanced filtering and querying.
        /// </returns>
        IQueryable<JobDone> GetAllAttached();

        /// <summary>
        /// Soft deletes a specific "Job Done" entry by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the "Job Done" entry to be deleted.</param>
        /// <returns>
        /// A boolean indicating whether the deletion operation was successful.
        /// </returns>
        Task<bool> SoftDeleteAsync(string id);
    }
}
