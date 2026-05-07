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
    using ElProApp.Web.Models.Material;

    /// <summary>
    /// Provides operations for retrieving material prices per building.
    /// </summary>
    public class BuildingMaterialPriceService : IBuildingMaterialPriceService
    {
        private readonly IRepository<BuildingMaterialPrice, object> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingMaterialPriceService"/> class.
        /// </summary>
        public BuildingMaterialPriceService(
            IRepository<BuildingMaterialPrice, object> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Retrieves the price of a material for a specific building.
        /// </summary>
        public async Task<decimal?> GetPriceAsync(Guid buildingId, Guid materialId)
        {
            var record = await repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);

            return record?.Price;
        }

        /// <summary>
        /// Sets or updates the price of a material for a specific building.
        /// </summary>
        public async Task SetPriceAsync(Guid buildingId, Guid materialId, decimal price)
        {
            var record = await repository
                .GetAllAttached()
                .FirstOrDefaultAsync(x =>
                    x.BuildingId == buildingId &&
                    x.MaterialId == materialId);

            if (record == null)
            {
                record = new BuildingMaterialPrice
                {
                    BuildingId = buildingId,
                    MaterialId = materialId,
                    Price = price
                };

                await repository.AddAsync(record);
            }
            else
            {
                record.Price = price;
            }

            await repository.SaveAsync();
        }

        /// <summary>
        /// Retrieves all material prices for a building.
        /// </summary>
        public async Task<IEnumerable<BuildingMaterialPrice>> GetByBuildingAsync(Guid buildingId) =>
            await repository
                .GetAllAttached()
                .Where(x => x.BuildingId == buildingId)
                .ToListAsync();
        

        /// <summary>
        /// Retrieves all prices for a given material across all buildings.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <returns>
        /// A collection of <see cref="BuildingMaterialPrice"/> for the specified material.
        /// </returns>
        public async Task<ICollection<BuildingMaterialPriceViewModel>> GetAllByMaterialIdAsync(Guid materialId) => 
            await repository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => x.MaterialId == materialId)
                .Select(x => new BuildingMaterialPriceViewModel
                {
                    BuildingId = x.BuildingId,
                    BuildingName = x.Building.Name,
                    MaterialId = x.MaterialId,
                    Price = x.Price
                })
                .ToListAsync();

        /// <summary>
        /// Retrieves prices based on building-material mappings.
        /// </summary>
        public async Task<Dictionary<Guid, decimal>> GetByJobDoneMaterialMapping(
            ICollection<JobDoneMaterialMapping> mappings)
        {
            if (mappings == null || !mappings.Any())
                return new Dictionary<Guid, decimal>();

            var buildingId = mappings.First().JobDone.BuildingId;

            var materialIds = mappings
                .Select(x => x.MaterialId)
                .Distinct()
                .ToList();

            return await repository
                .GetAllAttached()
                .Where(x => x.BuildingId == buildingId &&
                            materialIds.Contains(x.MaterialId))
                .ToDictionaryAsync(x => x.MaterialId, x => x.Price);
        }
    }
}
