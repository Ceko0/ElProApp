namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides operations for applying and rolling back material consumption
    /// related to job-done records.
    /// </summary>
    public class MaterialConsumptionService : IMaterialConsumptionService
    {
        private readonly IBuildingMaterialMappingService buildingMaterialService;
        private readonly IJobDoneMaterialMappingService jobDoneMaterialService;
        private readonly IMaterialService materialService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialConsumptionService"/> class.
        /// </summary>
        public MaterialConsumptionService(
            IBuildingMaterialMappingService buildingMaterialService,
            IJobDoneMaterialMappingService jobDoneMaterialService,
            IMaterialService materialService)
        {
            this.buildingMaterialService = buildingMaterialService;
            this.jobDoneMaterialService = jobDoneMaterialService;
            this.materialService = materialService;
        }

        /// <summary>
        /// Applies material consumption for a job-done record and decreases
        /// building material quantities accordingly.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materials">
        /// A dictionary where the key is the material identifier and the value is a tuple of quantity and price.
        /// </param>
        public async Task ApplyAsync(
            Guid jobDoneId,
            Guid buildingId,
            Dictionary<Guid, (decimal Quantity, decimal Price)> materials)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException("JobDoneId must not be empty.");

            if (buildingId == Guid.Empty)
                throw new ArgumentException("BuildingId must not be empty.");

            if (materials == null || materials.Count == 0)
                return;

            foreach (var (materialId, data) in materials)
            {
                if (data.Quantity <= 0)
                    continue;

                await jobDoneMaterialService.AddAsync(
                    jobDoneId.ToString(),
                    materialId.ToString(),
                    data.Quantity,
                    data.Price);

                await buildingMaterialService.DecreaseAsync(
                    buildingId,
                    materialId,
                    data.Quantity);
            }
        }

        /// <summary>
        /// Rolls back material consumption for a job-done record and restores
        /// building material quantities.
        /// </summary>
        /// <param name="jobDoneId">The job-done identifier.</param>
        public async Task RollbackAsync(Guid jobDoneId)
        {
            if (jobDoneId == Guid.Empty)
                throw new ArgumentException(
                    "JobDoneId must not be empty.", nameof(jobDoneId));

            var usedMaterials =
                await jobDoneMaterialService
                    .GetByJobDoneIdAsync(jobDoneId.ToString());

            if (usedMaterials == null || usedMaterials.Count == 0)
                return;

            foreach (var mapping in usedMaterials)
            {
                await buildingMaterialService.IncreaseAsync(
                    mapping.JobDone.BuildingId,
                    mapping.MaterialId,
                    mapping.Quantity);
            }

            await jobDoneMaterialService
                .RemoveByJobDoneIdAsync(jobDoneId.ToString());
        }
    }
}