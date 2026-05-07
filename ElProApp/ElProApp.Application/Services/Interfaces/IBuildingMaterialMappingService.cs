namespace ElProApp.Application.Services.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Provides operations for managing material quantities assigned to buildings.
    /// </summary>
    public interface IBuildingMaterialMappingService
    {
        /// <summary>
        /// Determines whether a building has at least the specified quantity of a material.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The required quantity.</param>
        /// <returns>True if enough quantity exists; otherwise, false.</returns>
        Task<bool> HasEnoughAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        /// <summary>
        /// Decreases the quantity of a material assigned to a building.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The quantity to decrease.</param>
        Task DecreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        /// <summary>
        /// Increases the quantity of a material assigned to a building.
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The quantity to increase.</param>
        Task IncreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        /// <summary>
        /// Sets exact quantity for a material in a building (overwrite).
        /// </summary>
        /// <param name="buildingId">The building identifier.</param>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="quantity">The new quantity value.</param>
        Task SetQuantityAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        /// <summary>
        /// Returns all building-material mappings as queryable collection.
        /// </summary>
        IQueryable<BuildingMaterialMapping> GetAllAttached();
    }
}