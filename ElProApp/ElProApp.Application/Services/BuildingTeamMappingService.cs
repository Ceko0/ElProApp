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

    /// <summary>
    /// Provides operations for managing building-team mappings.
    /// </summary>
    public class BuildingTeamMappingService : IBuildingTeamMappingService
    {
        private readonly IRepository<BuildingTeamMapping, object> buildingTeamMappingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingTeamMappingService"/> class.
        /// </summary>
        public BuildingTeamMappingService(
            IRepository<BuildingTeamMapping, object> buildingTeamMappingRepository)
        {
            this.buildingTeamMappingRepository = buildingTeamMappingRepository;
        }

        /// <summary>
        /// Creates a new building-team mapping.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when buildingId or teamId is empty.
        /// </exception>
        public async Task<BuildingTeamMapping> AddAsync(Guid buildingId, Guid teamId)
        {
            if (buildingId == Guid.Empty)
                throw new ArgumentException("BuildingId must not be empty.", nameof(buildingId));

            if (teamId == Guid.Empty)
                throw new ArgumentException("TeamId must not be empty.", nameof(teamId));

            var mapping = new BuildingTeamMapping
            {
                BuildingId = buildingId,
                TeamId = teamId
            };

            await buildingTeamMappingRepository.AddAsync(mapping);
            return mapping;
        }

        /// <summary>
        /// Retrieves all mappings for a given team.
        /// </summary>
        public async Task<ICollection<BuildingTeamMapping>> GetByTeamIdAsync(Guid teamId)
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all mappings for a given building.
        /// </summary>
        public async Task<ICollection<BuildingTeamMapping>> GetByBuildingIdAsync(Guid buildingId)
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.BuildingId == buildingId)
                .ToListAsync();

        /// <summary>
        /// Retrieves all mappings including related building and team entities.
        /// </summary>
        public async Task<ICollection<BuildingTeamMapping>> GetAllAttachedAsync()
            => await buildingTeamMappingRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Include(x => x.Team)
                .ToListAsync();

        /// <summary>
        /// Returns all building-team mappings as an attached query.
        /// </summary>
        public IQueryable<BuildingTeamMapping> GetAllAttached()
            => buildingTeamMappingRepository.GetAllAttached();

        /// <summary>
        /// Determines whether a mapping exists for the specified building and team.
        /// </summary>
        public bool Any(Guid buildingId, Guid teamId)
        {
            if (buildingId == Guid.Empty)
                throw new ArgumentException("BuildingId must not be empty.", nameof(buildingId));

            if (teamId == Guid.Empty)
                throw new ArgumentException("TeamId must not be empty.", nameof(teamId));

            return buildingTeamMappingRepository
                .GetAllAttached()
                .Any(x => x.BuildingId == buildingId && x.TeamId == teamId);
        }

        /// <summary>
        /// Removes an existing building-team mapping.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the mapping does not exist.
        /// </exception>
        public async Task<bool> RemoveAsync(BuildingTeamMapping mapping)
        {
            ArgumentNullException.ThrowIfNull(mapping);

            bool exists = await buildingTeamMappingRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.BuildingId == mapping.BuildingId &&
                    x.TeamId == mapping.TeamId);

            if (!exists)
                throw new InvalidOperationException("Building-team mapping not found.");

            return await buildingTeamMappingRepository
                .DeleteByCompositeKeyAsync(mapping.BuildingId, mapping.TeamId);
        }
    }
}
