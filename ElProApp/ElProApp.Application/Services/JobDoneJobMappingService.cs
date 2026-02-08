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
    /// Provides operations for managing job-done to job mappings.
    /// </summary>
    public class JobDoneJobMappingService : IJobDoneJobMappingService
    {
        private readonly IRepository<JobDoneJobMapping, object> jobDoneJobRepository;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneJobMappingService"/> class.
        /// </summary>
        public JobDoneJobMappingService(
            IRepository<JobDoneJobMapping, object> jobDoneJobRepository,
            IHelpMethodsService helpMethodsService)
        {
            this.jobDoneJobRepository = jobDoneJobRepository;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Creates a new job-done to job mapping.
        /// </summary>
        public async Task<JobDoneJobMapping> AddAsync(
            Guid jobDoneId,
            Guid jobId,
            decimal quantity)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            if (jobId == Guid.Empty)
                throw new ArgumentException(
                    "JobId must not be empty.", nameof(jobId));

            var jobDone = await helpMethodsService
                .GetAllJobDones()
                .FirstOrDefaultAsync(x =>
                    x.Id == jobDoneId && !x.IsDeleted)
                ?? throw new InvalidOperationException(
                    "JobDone not found or is deleted.");

            var job = await helpMethodsService
                .GetAllJobs()
                .FirstOrDefaultAsync(x =>
                    x.Id == jobId && !x.IsDeleted)
                ?? throw new InvalidOperationException(
                    "Job not found or is deleted.");

            var mapping = new JobDoneJobMapping
            {
                JobDoneId = jobDoneId,
                JobDone = jobDone,
                JobId = jobId,
                Job = job,
                Quantity = quantity
            };

            await jobDoneJobRepository.AddAsync(mapping);
            return mapping;
        }

        /// <summary>
        /// Retrieves all mappings for a given job identifier.
        /// </summary>
        public async Task<ICollection<JobDoneJobMapping>> GetByJobIdAsync(Guid jobId)
            => await jobDoneJobRepository
                .GetAllAttached()
                .Where(x => x.JobId == jobId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all mappings for a given job-done identifier.
        /// </summary>
        public async Task<ICollection<JobDoneJobMapping>> GetByJobDoneIdAsync(Guid jobDoneId)
            => await jobDoneJobRepository
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all mappings including related job and job-done entities.
        /// </summary>
        public async Task<ICollection<JobDoneJobMapping>> GetAllAttachedAsync()
            => await jobDoneJobRepository
                .GetAllAttached()
                .Include(x => x.Job)
                .Include(x => x.JobDone)
                .ToListAsync();

        /// <summary>
        /// Returns all job-done to job mappings as an attached query.
        /// </summary>
        public IQueryable<JobDoneJobMapping> GetAllAttached()
            => jobDoneJobRepository.GetAllAttached();

        /// <summary>
        /// Determines whether a mapping exists for the specified job-done and job.
        /// </summary>
        public bool Any(Guid jobDoneId, Guid jobId)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            if (jobId == Guid.Empty)
                throw new ArgumentException(
                    "JobId must not be empty.", nameof(jobId));

            return jobDoneJobRepository
                .GetAllAttached()
                .Any(x =>
                    x.JobDoneId == jobDoneId &&
                    x.JobId == jobId);
        }

        /// <summary>
        /// Removes an existing job-done to job mapping.
        /// </summary>
        public async Task<bool> RemoveAsync(Guid jobDoneId, Guid jobId)
        {
            bool exists = await jobDoneJobRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.JobDoneId == jobDoneId &&
                    x.JobId == jobId);

            if (!exists)
                throw new InvalidOperationException(
                    "Job-done job mapping not found.");

            return await jobDoneJobRepository
                .DeleteByCompositeKeyAsync(jobDoneId, jobId);
        }
    }
}
