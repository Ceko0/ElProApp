namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Job;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides application-level operations for managing job records (legacy).
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IRepository<Job, Guid> jobRepository;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobService"/> class.
        /// </summary>
        /// <param name="jobRepository">Repository for jobs.</param>
        /// <param name="helpMethodsService">Helper service.</param>
        public JobService(
            IRepository<Job, Guid> jobRepository,
            IHelpMethodsService helpMethodsService)
        {
            this.jobRepository = jobRepository;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Creates a new job.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <returns>The created job identifier.</returns>
        public async Task<string> AddAsync(JobInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity =
                AutoMapperConfig.MapperInstance.Map<Job>(model);

            await jobRepository.AddAsync(entity);
            return entity.Id.ToString();
        }

        /// <summary>
        /// Retrieves a job by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The job view model.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when job is not found.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when job is deleted.
        /// </exception>
        public async Task<JobViewModel> GetByIdAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobRepository.GetByIdAsync(validId)
                ?? throw new ArgumentException(
                    "Job not found.", nameof(id));

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Job is deleted.");

            return AutoMapperConfig.MapperInstance
                .Map<JobViewModel>(entity);
        }

        /// <summary>
        /// Retrieves a job edit model by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The edit model.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when job is not found or deleted.
        /// </exception>
        public async Task<JobEditInputModel> EditByIdAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobRepository.GetByIdAsync(validId)
                ?? throw new InvalidOperationException(
                    "Job not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Job is deleted.");

            return AutoMapperConfig.MapperInstance
                .Map<JobEditInputModel>(entity);
        }

        /// <summary>
        /// Updates an existing job.
        /// </summary>
        /// <param name="model">The edit model.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when job is not found or deleted.
        /// </exception>
        public async Task<bool> EditByModelAsync(JobEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await jobRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException(
                    "Job not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Job is deleted.");

            AutoMapperConfig.MapperInstance.Map(model, entity);
            await jobRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// Retrieves all non-deleted jobs.
        /// </summary>
        /// <returns>A collection of jobs.</returns>
        public async Task<ICollection<JobViewModel>> GetAllAsync()
            => await jobRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<JobViewModel>()
                .ToListAsync();

        /// <summary>
        /// Returns all attached, non-deleted jobs.
        /// </summary>
        /// <returns>Queryable jobs.</returns>
        public IQueryable<Job> GetAllAttached()
            => jobRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Soft deletes a job by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            return await jobRepository
                .SoftDeleteAsync(validId);
        }
    }
}