namespace ElProApp.Application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Provides operations for retrieving material prices per building.
    /// </summary>
    public interface IBuildingMaterialPriceService
    {
        /// <summary>
        /// Retrieves the price of a material for a specific building.
        /// </summary>
        Task<decimal?> GetPriceAsync(Guid buildingId, Guid materialId);

        /// <summary>
        /// Sets or updates the price of a material for a specific building.
        /// </summary>
        Task SetPriceAsync(Guid buildingId, Guid materialId, decimal price);

        /// <summary>
        /// Retrieves all material prices for a building.
        /// </summary>
        Task<IEnumerable<BuildingMaterialPrice>> GetByBuildingAsync(Guid buildingId);
    }
}
