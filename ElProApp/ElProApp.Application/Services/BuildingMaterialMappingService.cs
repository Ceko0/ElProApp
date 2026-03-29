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
        /// <param name="repository">The repository used for data access.</param>
        public BuildingMaterialMappingService(
            IRepository<BuildingMaterialMapping, object> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Retrieves all building-material mappings as an attached query.
        /// </summary>
        /// <returns>An <see cref="IQueryable{BuildingMaterialMapping}"/> for further querying.</returns>
        public IQueryable<BuildingMaterialMapping> GetAllAttached()
            => repository.GetAllAttached();

        /// <summary>
        /// Determines whether a building has at least the specified quantity of a material.
        /// </summary>
        /// <param name="buildingId">The identifier of the building.</param>
        /// <param name="materialId">The identifier of the material.</param>
        /// <param name="quantity">The required quantity.</param>
        /// <returns>True if the required quantity is available; otherwise, false.</returns>
        /// <remarks>
        /// This method is informational only and does not prevent operations.
        /// Quantities in the system are allowed to become negative.
        /// </remarks>
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
        /// <param name="buildingId">The identifier of the building.</param>
        /// <param name="materialId">The identifier of the material.</param>
        /// <param name="quantity">The quantity to decrease.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the material is not assigned to the specified building.
        /// </exception>
        /// <remarks>
        /// This operation allows the quantity to become negative as part of system control and auditing.
        /// </remarks>
        public async Task DecreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity)
        {
            var record = await GetRecordAsync(buildingId, materialId)
                ?? throw new InvalidOperationException(
                    "Material is not assigned to the specified building.");

            record.Quantity -= quantity;
            record.LastUpdated = DateTime.UtcNow;

            await repository.SaveAsync();
        }

        /// <summary>
        /// Increases the quantity of a material assigned to a building.
        /// </summary>
        /// <param name="buildingId">The identifier of the building.</param>
        /// <param name="materialId">The identifier of the material.</param>
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

        /// <summary>
        /// Retrieves a building-material mapping record for the specified building and material.
        /// </summary>
        /// <param name="buildingId">The identifier of the building.</param>
        /// <param name="materialId">The identifier of the material.</param>
        /// <returns>
        /// The <see cref="BuildingMaterialMapping"/> if found; otherwise, null.
        /// </returns>
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