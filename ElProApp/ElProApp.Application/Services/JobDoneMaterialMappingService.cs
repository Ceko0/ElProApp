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
    /// Provides operations for managing job-done to material mappings.
    /// </summary>
    public class JobDoneMaterialMappingService : IJobDoneMaterialMappingService
    {
        private readonly IRepository<JobDoneMaterialMapping, object> jobDoneMaterialMappingRepository;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneMaterialMappingService"/> class.
        /// </summary>
        public JobDoneMaterialMappingService(
            IRepository<JobDoneMaterialMapping, object> jobDoneMaterialMappingRepository,
            IHelpMethodsService helpMethodsService)
        {
            this.jobDoneMaterialMappingRepository = jobDoneMaterialMappingRepository;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Creates a new job-done material mapping.
        /// </summary>
        public async Task<JobDoneMaterialMapping> AddAsync(
            string jobDoneId,
            string materialId,
            decimal quantity,
            decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(jobDoneId))
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            if (string.IsNullOrWhiteSpace(materialId))
                throw new ArgumentException(
                    "MaterialId must not be empty.", nameof(materialId));

            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException(
                    "Unit price cannot be negative.", nameof(unitPrice));

            Guid jobDoneGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);

            Guid materialGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(materialId);

            var mapping = new JobDoneMaterialMapping
            {
                JobDoneId = jobDoneGuidId,
                MaterialId = materialGuidId,
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            await jobDoneMaterialMappingRepository.AddAsync(mapping);
            return mapping;
        }

        /// <summary>
        /// Retrieves all material mappings for a specific job-done identifier.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetByJobDoneIdAsync(
            string jobDoneId)
        {
            Guid jobDoneGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);

            return await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneGuidId && !x.IsDeleted)
                .Include(x => x.Material)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all job-done mappings for a specific material identifier.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetByMaterialIdAsync(
            string materialId)
        {
            Guid materialGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(materialId);

            return await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => x.MaterialId == materialGuidId && !x.IsDeleted)
                .Include(x => x.JobDone)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all mappings including related entities.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetAllAttachedAsync()
            => await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Include(x => x.JobDone)
                .Include(x => x.Material)
                .Where(x => !x.IsDeleted)
                .ToListAsync();

        /// <summary>
        /// Returns queryable mappings.
        /// </summary>
        public IQueryable<JobDoneMaterialMapping> GetAllAttached()
            => jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Checks whether a mapping exists.
        /// </summary>
        public bool Any(string jobDoneId, string materialId)
        {
            Guid jobDoneGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);

            Guid materialGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(materialId);

            return jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Any(x =>
                    x.JobDoneId == jobDoneGuidId &&
                    x.MaterialId == materialGuidId &&
                    !x.IsDeleted);
        }

        /// <summary>
        /// Removes a job-done material mapping.
        /// </summary>
        public async Task<bool> RemoveAsync(JobDoneMaterialMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            return await jobDoneMaterialMappingRepository
                .DeleteByCompositeKeyAsync(
                    mapping.JobDoneId,
                    mapping.MaterialId);
        }

        /// <summary>
        /// Soft deletes all material mappings for a job-done record.
        /// </summary>
        public async Task RemoveByJobDoneIdAsync(string jobDoneId)
        {
            Guid jobDoneGuidId =
                helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);

            var mappings = await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x =>
                    x.JobDoneId == jobDoneGuidId &&
                    !x.IsDeleted)
                .ToListAsync();

            if (!mappings.Any())
                return;

            foreach (var mapping in mappings)
            {
                mapping.IsDeleted = true;
                mapping.DeletedOn = DateTime.UtcNow;
            }

            await jobDoneMaterialMappingRepository.SaveAsync();
        }
    }
}
