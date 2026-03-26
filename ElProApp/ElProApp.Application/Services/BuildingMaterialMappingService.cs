namespace ElProApp.Application.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing material quantities assigned to buildings.
    /// </summary>
    public class BuildingMaterialMappingService
        : IBuildingMaterialMappingService
    {
        private readonly IRepository<BuildingMaterialMapping, object> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingMaterialMappingService"/> class.
        /// </summary>
        public BuildingMaterialMappingService(
            IRepository<BuildingMaterialMapping, object> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Retrieves all building-material mappings as an attached query.
        /// </summary>
        public IQueryable<BuildingMaterialMapping> GetAllAttached()
            => repository.GetAllAttached();

        /// <summary>
        /// Determines whether a building has at least the specified quantity of a material.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The required quantity.</param>
        /// <returns>True if the quantity is available; otherwise false.</returns>
        public async Task<bool> HasEnoughAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await GetRecordAsync(buildingId, materialId);
            return record != null && record.Quantity >= quantity;
        }

        /// <summary>
        /// Decreases the quantity of a material assigned to a building.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The quantity to decrease.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the material is not available or the quantity is insufficient.
        /// </exception>
        public async Task DecreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await GetRecordAsync(buildingId, materialId)
                ?? throw new InvalidOperationException(
                    "Material is not available in the specified building.");
                       
            record.Quantity -= quantity;
            record.LastUpdated = DateTime.UtcNow;

            await repository.SaveAsync();
        }

        /// <summary>
        /// Increases the quantity of a material assigned to a building.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The quantity to increase.</param>
        public async Task IncreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await GetRecordAsync(buildingId, materialId);

            if (record == null)
            {
                record = new BuildingMaterialMapping
                {
                    BuildingId = buildingId,
                    MaterialId = materialId,
                    Quantity = quantity,
                    LastUpdated = DateTime.UtcNow
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

        private Task<BuildingMaterialMapping?> GetRecordAsync(
            Guid buildingId,
            Guid materialId)
            => repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);
    }
}
