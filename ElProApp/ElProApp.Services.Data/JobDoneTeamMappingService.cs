﻿namespace ElProApp.Services.Data
{ 
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;

    /// <summary>
    /// Service class for managing job done team mappings. Provides functionality to retrieve, add, and remove mappings between Teams and job done records.
    /// </summary>
    public class JobDoneTeamMappingService(IRepository<JobDoneTeamMapping, object> jobDoneTeamMappingRepository) : IJobDoneTeamMappingService
    {
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
        {
            if (Id == Guid.Empty) throw new ArgumentNullException(nameof(Id), "Invalid teamId.");

            var model = await jobDoneTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == Id)
                .ToListAsync();
            return model;
        }

        public async Task<JobDoneTeamMapping> GetByJobDoneIdAsync(Guid Id)
         {
            if (Id == Guid.Empty) throw new ArgumentNullException(nameof(Id), "Invalid jobDoneId.");

            var model = await jobDoneTeamMappingRepository
            .GetAllAttached()
            .Include(x => x.Team)
            .Include( x => x.JobDone)
            .ThenInclude(jd => jd.Building)
            .FirstOrDefaultAsync(x => x.JobDoneId == Id);
            
            return model;
        }

        /// <summary>
        /// Adds a new job done team mapping for the given job done ID and team ID.
        /// </summary>
        /// <param name="jobDoneId">The ID of the job done record.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>The newly created <see cref="JobDoneTeamMapping"/> object.</returns>
        public async Task<JobDoneTeamMapping> AddAsync(Guid jobDoneId, Guid teamId)
        {
            if (jobDoneId == Guid.Empty) throw new ArgumentNullException(nameof(jobDoneId), "Invalid jobDoneId.");
            if (teamId == Guid.Empty) throw new ArgumentNullException(nameof(teamId), "Invalid teamId.");

            var model = new JobDoneTeamMapping()
            {
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
            var model = jobDoneTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneId && x.TeamId == teamId);

            return model.Any();
        }

        /// <summary>
        /// Removes a specific job done team mapping.
        /// </summary>
        /// <param name="mapping">The job done team mapping to remove.</param>
        /// <returns>True if the mapping was successfully removed; otherwise, false.</returns>
        public async Task<bool> RemoveAsync(JobDoneTeamMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping), "Mapping cannot be null.");

            return await jobDoneTeamMappingRepository
                .DeleteByCompositeKeyAsync(mapping.JobDoneId, mapping.TeamId);
        }
    }
}
