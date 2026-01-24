namespace ElProApp.Services.Data
{
    using ElProApp.Services.Data.Interfaces;

    public class MaterialConsumptionService : IMaterialConsumptionService
    {
        private readonly IBuildingMaterialMappingService buildingMaterialService;
        private readonly IJobDoneMaterialMappingService jobDoneMaterialService;
        private readonly IMaterialService materialService;

        public MaterialConsumptionService(
            IBuildingMaterialMappingService buildingMaterialService,
            IJobDoneMaterialMappingService jobDoneMaterialService,
            IMaterialService materialService)
        {
            this.buildingMaterialService = buildingMaterialService;
            this.jobDoneMaterialService = jobDoneMaterialService;
            this.materialService = materialService;
        }

        public async Task ApplyAsync(
            Guid jobDoneId,
            Guid buildingId,
            Dictionary<Guid, decimal> materials)
        {
            foreach (var (materialId, qty) in materials)
            {
                if (qty <= 0) continue;

                var material = await materialService.GetByIdAsync(materialId.ToString());

                await jobDoneMaterialService.AddAsync(
                    jobDoneId.ToString(),
                    materialId.ToString(),
                    qty,
                    material.Price
                );

                await buildingMaterialService.DecreaseAsync(
                    buildingId,
                    materialId,
                    qty
                );
            }
        }

        public async Task RollbackAsync(Guid jobDoneId)
        {
            var usedMaterials =
                await jobDoneMaterialService.GetByJobDoneIdAsync(jobDoneId.ToString());

            foreach (var m in usedMaterials)
            {
                await buildingMaterialService.IncreaseAsync(
                    m.JobDone.BuildingId,
                    m.MaterialId,
                    m.Quantity
                );
            }

            await jobDoneMaterialService.RemoveByJobDoneIdAsync(jobDoneId.ToString());
        }
    }

}
