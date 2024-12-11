namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Service for managing building-team mappings.
    /// </summary>
    public class BuildingTeamMappingService(IRepository<BuildingTeamMapping, Guid> buildingTeamMappingRepository)
        : IBuildingTeamMappingService
    {

        /// <summary>
        /// Adds a new building-team mapping.
        /// </summary>
        /// <param name="buildingId">The ID of the building.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>The newly created <see cref="BuildingTeamMapping"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either the building ID or team ID is invalid (Guid.Empty).</exception>
        public async Task<BuildingTeamMapping> AddAsync(Guid buildingId, Guid teamId)
        {
            if (buildingId == Guid.Empty) throw new ArgumentNullException("Invalid buildingId");
            if (teamId == Guid.Empty) throw new ArgumentNullException("Invalid TeamId");

            var buildingTeamMapping = new BuildingTeamMapping()
            {
                BuildingId = buildingId,
                TeamId = teamId
            };
            await buildingTeamMappingRepository.AddAsync(buildingTeamMapping);

            return buildingTeamMapping;
        }


        /// <summary>
        /// Retrieves all building-team mappings for a specific team.
        /// </summary>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>A collection of <see cref="BuildingTeamMapping"/> objects for the specified team.</returns>
        public async Task<ICollection<BuildingTeamMapping>> GetByTeamIdAsync(Guid teamId)
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all building-team mappings for a specific building.
        /// </summary>
        /// <param name="buildingId">The ID of the building.</param>
        /// <returns>A collection of <see cref="BuildingTeamMapping"/> objects for the specified building.</returns>
        public async Task<ICollection<BuildingTeamMapping>> GetByBuildingIdAsync(Guid buildingId)
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.BuildingId == buildingId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all building-team mappings, including related entities.
        /// </summary>
        /// <returns>A collection of <see cref="BuildingTeamMapping"/> objects with their related <see cref="Building"/> and <see cref="Team"/> entities.</returns>
        public async Task<ICollection<BuildingTeamMapping>> GetAllAttachedAsync()
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Include(x => x.Team)
                .ToListAsync();

        /// <summary>
        /// Retrieves all building-team mappings.
        /// </summary>
        /// <returns>A queryable collection of <see cref="BuildingTeamMapping"/> objects.</returns>
        public IQueryable<BuildingTeamMapping> GetAllAttached()
            => buildingTeamMappingRepository
                .GetAllAttached();

        /// <summary>
        /// Checks if a building-team mapping exists for a given building and team.
        /// </summary>
        /// <param name="buildingId">The ID of the building.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>True if the mapping exists, otherwise false.</returns>
        public bool Any(Guid buildingId, Guid teamId)
        {
            if (buildingId == Guid.Empty || teamId == Guid.Empty)
                throw new ArgumentException("BuildingId and TeamId must not be empty.");

            var model = buildingTeamMappingRepository.GetAllAttached().Where(x => x.TeamId == teamId && x.BuildingId == buildingId);
            return model.Any();
        }


        /// <summary>
        /// Removes a building-team mapping.
        /// </summary>
        /// <param name="mapping">The <see cref="BuildingTeamMapping"/> to remove.</param>
        /// <returns>True if the mapping was removed successfully, otherwise false.</returns>
        public async Task<bool> RemoveAsync(BuildingTeamMapping mapping)
        {
            var mappingExists = await buildingTeamMappingRepository
                .GetAllAttached()
                .AnyAsync(x => x.BuildingId == mapping.BuildingId && x.TeamId == mapping.TeamId);

            if (!mappingExists) throw new InvalidOperationException("Mapping not found.");

            return await buildingTeamMappingRepository.DeleteByCompositeKeyAsync(mapping.BuildingId, mapping.TeamId);
        }

    }
}
