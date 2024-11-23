namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IJobDoneTeamMappingService
    {
        /// <summary>
        /// Retrieves all "Job Done"-team mappings as an asynchronous collection.
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="JobDoneTeamMapping"/> objects.
        /// </returns>
        Task<ICollection<JobDoneTeamMapping>> GetAllAsync();

        /// <summary>
        /// Retrieves all "Job Done"-team mappings as a queryable collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="JobDoneTeamMapping"/> objects for advanced querying.
        /// </returns>
        IQueryable<JobDoneTeamMapping> GetAllAttached();

        /// <summary>
        /// Retrieves all "Job Done"-team mappings for a specific team by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the team.</param>
        /// <returns>
        /// A collection of <see cref="JobDoneTeamMapping"/> objects associated with the specified team.
        /// </returns>
        Task<ICollection<JobDoneTeamMapping>> GetByTeamIdAsync(Guid id);

        /// <summary>
        /// Adds a new mapping between a "Job Done" entry and a team.
        /// </summary>
        /// <param name="jobDoneId">The unique ID of the "Job Done" entry.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// The created <see cref="JobDoneTeamMapping"/> object representing the new mapping.
        /// </returns>
        Task<JobDoneTeamMapping> AddAsync(Guid jobDoneId, Guid teamId);

        /// <summary>
        /// Checks if a mapping exists between a "Job Done" entry and a team.
        /// </summary>
        /// <param name="jobDoneId">The unique ID of the "Job Done" entry.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// True if a mapping between the specified "Job Done" entry and team exists, otherwise false.
        /// </returns>
        bool Any(Guid jobDoneId, Guid teamId);

        /// <summary>
        /// Removes a specific mapping between a "Job Done" entry and a team.
        /// </summary>
        /// <param name="mapping">
        /// The <see cref="JobDoneTeamMapping"/> object representing the mapping to be removed.
        /// </param>
        /// <returns>
        /// A boolean indicating whether the removal operation was successful.
        /// </returns>
        Task<bool> RemoveAsync(JobDoneTeamMapping mapping);
    }
}
