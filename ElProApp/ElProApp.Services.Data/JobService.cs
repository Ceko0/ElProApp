namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;
    using Mapping;
    using Web.Models.Job;

    /// <summary>
    /// Service class for managing job records. Provides functionality to add, edit, retrieve, and delete job records.
    /// </summary>
    public class JobService(IRepository<Job, Guid> jobRepository,
                            IHelpMethodsService helpMethodsService) :
                            IJobService
    {
        /// <summary>
        /// Adds a new job based on the provided input model.
        /// </summary>
        /// <param name="model">The input model containing job details.</param>
        /// <returns>The ID of the newly created job as a string.</returns>
        public async Task<string> AddAsync(JobInputModel model)
        {
            var entity = AutoMapperConfig.MapperInstance.Map<Job>(model);

            await jobRepository.AddAsync(entity);
            return entity.Id.ToString();
        }

        /// <summary>
        /// Retrieves a job by its ID.
        /// </summary>
        /// <param name="id">The ID of the job to retrieve.</param>
        /// <returns>A <see cref="JobViewModel"/> representation of the job.</returns>
        /// <exception cref="ArgumentException">Thrown if the job cannot be found.</exception>
        public async Task<JobViewModel> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobRepository.GetByIdAsync(validId);

            if (entity != null && !entity.IsDeleted)
            {
                var model = AutoMapperConfig.MapperInstance.Map<JobViewModel>(entity);
                return model;
            }

            throw new ArgumentException("Missing or deleted entity.");
        }

        /// <summary>
        /// Retrieves the job data for editing based on its ID.
        /// </summary>
        /// <param name="id">The ID of the job to retrieve for editing.</param>
        /// <returns>A <see cref="JobEditInputModel"/> with the job details for editing.</returns>
        public async Task<JobEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var entity = await jobRepository.GetByIdAsync(validId);

            if (entity == null || entity.IsDeleted)
                throw new InvalidOperationException("Job is deleted or not found.");

            return AutoMapperConfig.MapperInstance.Map<JobEditInputModel>(entity);
        }

        /// <summary>
        /// Edits a job record based on the provided model.
        /// </summary>
        /// <param name="model">The model containing updated job details.</param>
        /// <returns>True if the job was successfully updated, otherwise false.</returns>
        public async Task<bool> EditByModelAsync(JobEditInputModel model)
        {
            try
            {
                var entity = await jobRepository.GetByIdAsync(model.Id);
                if (entity == null || entity.IsDeleted)
                    throw new InvalidOperationException("Job is deleted or not found.");
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await jobRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all jobs.
        /// </summary>
        /// <returns>A collection of all jobs as <see cref="JobViewModel"/>.</returns>
        public async Task<ICollection<JobViewModel>> GetAllAsync()
        {
            return await jobRepository.GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<JobViewModel>()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all jobs as IQueryable, attached with relevant data.
        /// </summary>
        /// <returns>An <see cref="IQueryable{Job}"/> representing all jobs.</returns>
        public IQueryable<Job> GetAllAttached()
            => jobRepository
            .GetAllAttached()
            .Where(x => !x.IsDeleted);

        /// <summary>
        /// Soft deletes a job by its ID.
        /// </summary>
        /// <param name="id">The ID of the job to soft delete.</param>
        /// <returns>True if the job was successfully soft deleted, otherwise false.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
                bool isDeleted = await jobRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch
            {
                return false;
            }
        }
    }
}
