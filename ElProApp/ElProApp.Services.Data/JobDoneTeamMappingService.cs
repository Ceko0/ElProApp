namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.JobDone;
    using Microsoft.EntityFrameworkCore;
    using System.Xml.Linq;

    /// <summary>
    /// Service class for managing job done team mappings. Provides functionality to retrieve, add, and remove mappings between teams and job done records.
    /// </summary>
    public class JobDoneTeamMappingService(IRepository<JobDoneTeamMapping, Guid> _jobDoneTeamMappingRepository) : IJobDoneTeamMappingService
    {
        private readonly IRepository<JobDoneTeamMapping, Guid> jobDoneTeamMappingRepository = _jobDoneTeamMappingRepository;

        /// <summary>
        /// Retrieves all job done team mappings.
        /// </summary>
        /// <returns>A collection of <see cref="JobDoneTeamMapping"/>.</returns>
        public async Task<ICollection<JobDoneTeamMapping>> GetAllAsync()
            => await jobDoneTeamMappingRepository
                .GetAllAttached()
                .ToListAsync();

        /// <summary>
        /// Retrieves all attached job done team mappings as a queryable collection.
        /// </summary>
        /// <returns>An <see cref="IQueryable"/> collection of <see cref="JobDoneTeamMapping"/>.</returns>
        public IQueryable<JobDoneTeamMapping> GetAllAttached()
            => jobDoneTeamMappingRepository
                .GetAllAttached();

        /// <summary>
        /// Retrieves all job done team mappings by team ID.
        /// </summary>
        /// <param name="Id">The ID of the team.</param>
        /// <returns>A collection of <see cref="JobDoneTeamMapping"/> related to the given team ID.</returns>
        public async Task<ICollection<JobDoneTeamMapping>> GetByTeamIdAsync(Guid Id)
            => await jobDoneTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == Id)
                .ToListAsync();

        /// <summary>
        /// Adds a new job done team mapping for the given job done ID and team ID.
        /// </summary>
        /// <param name="jobDoneId">The ID of the job done record.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>The newly created <see cref="JobDoneTeamMapping"/> object.</returns>
        public async Task<JobDoneTeamMapping> AddAsync(Guid jobDoneId, Guid teamId)
        {
            var model = new JobDoneTeamMapping()
            {
                Id = Guid.NewGuid(),
                JobDoneId = jobDoneId,
                TeamId = teamId
            };

            await jobDoneTeamMappingRepository.AddAsync(model);

            return model;
        }

        /// <summary>
        /// Checks if a mapping between the specified job done ID and team ID already exists.
        /// </summary>
        /// <param name="jobDoneId">The ID of the job done record.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>True if the mapping exists; otherwise, false.</returns>
        public bool Any(Guid jobDoneId, Guid teamId)
        {
            var model = jobDoneTeamMappingRepository.GetAllAttached().Where(x => x.JobDoneId == jobDoneId && x.TeamId == teamId);

            return model.Any();
        }

        /// <summary>
        /// Removes a specific job done team mapping.
        /// </summary>
        /// <param name="mapping">The job done team mapping to remove.</param>
        /// <returns>True if the mapping was successfully removed; otherwise, false.</returns>
        public async Task<bool> RemoveAsync(JobDoneTeamMapping mapping)
            => await jobDoneTeamMappingRepository.DeleteByCompositeKeyAsync(mapping.JobDoneId, mapping.TeamId);
    }
}
