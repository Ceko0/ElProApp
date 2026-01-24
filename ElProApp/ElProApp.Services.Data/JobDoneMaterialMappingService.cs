namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Service for managing job-done material mappings.
    /// </summary>
    public class JobDoneMaterialMappingService(
        IRepository<JobDoneMaterialMapping, object> jobDoneMaterialMappingRepository, IHelpMethodsService helpMethodsService)
        : IJobDoneMaterialMappingService
    {
        /// <summary>
        /// Adds a new job-done material mapping.
        /// </summary>
        public async Task<JobDoneMaterialMapping> AddAsync(
    string jobDoneId,
    string materialId,
    decimal quantity,
    decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(jobDoneId))
                throw new ArgumentNullException(nameof(jobDoneId));

            if (string.IsNullOrWhiteSpace(materialId))
                throw new ArgumentNullException(nameof(materialId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            var jobDoneGuidId = helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);
            var materialGuidId = helpMethodsService.ConvertAndTestIdToGuid(materialId);

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
        /// Retrieves all material mappings for a specific job.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetByJobDoneIdAsync(string jobDoneId)
            => await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => x.JobDoneId == helpMethodsService.ConvertAndTestIdToGuid(jobDoneId))
                .Include(x => x.Material)
                .ToListAsync();

        /// <summary>
        /// Retrieves all job mappings for a specific material.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetByMaterialIdAsync(string materialId)
            => await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => x.MaterialId == helpMethodsService.ConvertAndTestIdToGuid(materialId))
                .Include(x => x.JobDone)
                .ToListAsync();

        /// <summary>
        /// Retrieves all mappings including related entities.
        /// </summary>
        public async Task<ICollection<JobDoneMaterialMapping>> GetAllAttachedAsync()
            => await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Include(x => x.JobDone)
                .Include(x => x.Material)
                .ToListAsync();

        /// <summary>
        /// Returns queryable mappings.
        /// </summary>
        public IQueryable<JobDoneMaterialMapping> GetAllAttached()
            => jobDoneMaterialMappingRepository.GetAllAttached();

        /// <summary>
        /// Checks if mapping exists.
        /// </summary>
        public bool Any(string jobDoneId, string materialId)
        {
            if (jobDoneId == string.Empty || materialId == string.Empty)
                throw new ArgumentException("Ids must not be empty.");

            var jobDoneGuidId = helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);
            var materialGuidId = helpMethodsService.ConvertAndTestIdToGuid(materialId); 

            return jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Any(x => x.JobDoneId == jobDoneGuidId && x.MaterialId == materialGuidId);
        }

        /// <summary>
        /// Removes a job-done material mapping.
        /// </summary>
        public async Task<bool> RemoveAsync(JobDoneMaterialMapping mapping)
        {
            var exists = await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.JobDoneId == mapping.JobDoneId &&
                    x.MaterialId == mapping.MaterialId);

            if (!exists)
                throw new InvalidOperationException("Mapping not found.");

            return await jobDoneMaterialMappingRepository
                .DeleteByCompositeKeyAsync(mapping.JobDoneId, mapping.MaterialId);
        }

        public async Task RemoveByJobDoneIdAsync(string jobDoneId)
        {
            if (string.IsNullOrWhiteSpace(jobDoneId))
                throw new ArgumentNullException(nameof(jobDoneId));

            var jobDoneGuidId = helpMethodsService.ConvertAndTestIdToGuid(jobDoneId);

            var mappings = await jobDoneMaterialMappingRepository
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneGuidId && !x.IsDeleted)
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
