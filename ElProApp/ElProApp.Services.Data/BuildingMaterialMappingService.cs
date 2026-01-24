namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;

    public class BuildingMaterialMappingService
    : IBuildingMaterialMappingService
    {
        private readonly IRepository<BuildingMaterialMapping, object> repository;

        public BuildingMaterialMappingService(
            IRepository<BuildingMaterialMapping, object> repository)
        {
            this.repository = repository;
        }

        public IQueryable<BuildingMaterialMapping> GetAllAttached()
            => repository.GetAllAttached();

        public async Task<bool> HasEnoughAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);

            return record != null && record.Quantity >= quantity;
        }

        public async Task DecreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);

            if (record == null)
                throw new InvalidOperationException("Material not available in this building");

            if (record.Quantity < quantity)
                throw new InvalidOperationException("Not enough material in building");

            record.Quantity -= quantity;
            record.LastUpdated = DateTime.UtcNow;

            await repository.SaveAsync();
        }

        public async Task IncreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);

            if (record == null)
            {
                record = new BuildingMaterialMapping
                {
                    BuildingId = buildingId,
                    MaterialId = materialId,
                    Quantity = quantity
                };

                await repository.AddAsync(record);
            }
            else
            {
                record.Quantity += quantity;
                record.LastUpdated = DateTime.UtcNow;
            }

            await repository.SaveAsync();
        }
    }

}
