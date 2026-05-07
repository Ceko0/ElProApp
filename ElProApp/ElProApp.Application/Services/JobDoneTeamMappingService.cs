namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing job-done to team mappings.
    /// </summary>
    public class JobDoneTeamMappingService : IJobDoneTeamMappingService
    {
        private readonly IRepository<JobDoneTeamMapping, object> jobDoneTeamMappingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneTeamMappingService"/> class.
        /// </summary>
        public JobDoneTeamMappingService(
            IRepository<JobDoneTeamMapping, object> jobDoneTeamMappingRepository)
        {
            this.jobDoneTeamMappingRepository = jobDoneTeamMappingRepository;
        }

        /// <summary>
        /// Retrieves all job-done team mappings.
        /// </summary>
        public async Task<ICollection<JobDoneTeamMapping>> GetAllAsync()
            => await jobDoneTeamMappingRepository
                .GetAllAttached()
                .ToListAsync();

        /// <summary>
        /// Returns all job-done team mappings as an attached query.
        /// </summary>
        public IQueryable<JobDoneTeamMapping> GetAllAttached()
            => jobDoneTeamMappingRepository.GetAllAttached();

        /// <summary>
        /// Retrieves all mappings for a given team identifier.
        /// </summary>
        public async Task<ICollection<JobDoneTeamMapping>> GetByTeamIdAsync(Guid teamId)
        {
            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            return await jobDoneTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the job-done team mapping for a given job-done identifier.
        /// </summary>
        public async Task<JobDoneTeamMapping?> GetByJobDoneIdAsync(Guid jobDoneId)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            return await jobDoneTeamMappingRepository
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.JobDone)
                .ThenInclude(jd => jd.Building)
                .FirstOrDefaultAsync(x => x.JobDoneId == jobDoneId);
        }

        /// <summary>
        /// Creates a new job-done to team mapping.
        /// </summary>
        public async Task<JobDoneTeamMapping> AddAsync(Guid jobDoneId, Guid teamId)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            var mapping = new JobDoneTeamMapping
            {
                JobDoneId = jobDoneId,
                TeamId = teamId
            };

            await jobDoneTeamMappingRepository.AddAsync(mapping);
            return mapping;
        }

        /// <summary>
        /// Determines whether a mapping exists for the specified job-done and team.
        /// </summary>
        public bool Any(Guid jobDoneId, Guid teamId)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            return jobDoneTeamMappingRepository
                .GetAllAttached()
                .Any(x =>
                    x.JobDoneId == jobDoneId &&
                    x.TeamId == teamId);
        }

        /// <summary>
        /// Removes an existing job-done to team mapping.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the mapping does not exist.
        /// </exception>
        public async Task<bool> RemoveAsync(JobDoneTeamMapping mapping)
        {
            ArgumentNullException.ThrowIfNull(mapping);

            bool exists = await jobDoneTeamMappingRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.JobDoneId == mapping.JobDoneId &&
                    x.TeamId == mapping.TeamId);

            if (!exists)
                throw new InvalidOperationException(
                    "Job-done team mapping not found.");

            return await jobDoneTeamMappingRepository
                .DeleteByCompositeKeyAsync(
                    mapping.JobDoneId,
                    mapping.TeamId);
        }
    }
}
