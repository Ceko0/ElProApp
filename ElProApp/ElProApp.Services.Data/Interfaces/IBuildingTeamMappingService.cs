namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IBuildingTeamMappingService
    {
        /// <summary>
        /// Adds a new mapping between a building and a team.
        /// This establishes a relationship indicating that a team is associated with a specific building.
        /// </summary>
        /// <param name="buildingId">The unique ID of the building.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// The created <see cref="BuildingTeamMapping"/> object containing the details of the mapping.
        /// </returns>
        Task<BuildingTeamMapping> AddAsync(Guid buildingId, Guid teamId);

        /// <summary>
        /// Retrieves all mappings associated with a specific team.
        /// Useful for identifying all buildings where the given team is assigned.
        /// </summary>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// A collection of <see cref="BuildingTeamMapping"/> objects associated with the specified team.
        /// Returns an empty collection if no mappings exist.
        /// </returns>
        Task<ICollection<BuildingTeamMapping>> GetByTeamIdAsync(Guid teamId);

        /// <summary>
        /// Retrieves all mappings associated with a specific building.
        /// Useful for identifying all teams assigned to the given building.
        /// </summary>
        /// <param name="buildingId">The unique ID of the building.</param>
        /// <returns>
        /// A collection of <see cref="BuildingTeamMapping"/> objects associated with the specified building.
        /// Returns an empty collection if no mappings exist.
        /// </returns>
        Task<ICollection<BuildingTeamMapping>> GetByBuildingIdAsync(Guid buildingId);

        /// <summary>
        /// Retrieves all building-team mappings as an asynchronous collection.
        /// Use this method when working with in-memory lists or non-queryable data.
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="BuildingTeamMapping"/> objects in the system.
        /// </returns>
        Task<ICollection<BuildingTeamMapping>> GetAllAttachedAsync();

        /// <summary>
        /// Retrieves all building-team mappings as a queryable collection.
        /// This method is suitable for scenarios requiring filtering, sorting, or other query operations.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="BuildingTeamMapping"/> objects.
        /// </returns>
        IQueryable<BuildingTeamMapping> GetAllAttached();

        /// <summary>
        /// Checks if a specific mapping exists between a building and a team.
        /// </summary>
        /// <param name="buildingId">The unique ID of the building.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// True if a mapping exists, otherwise false.
        /// </returns>
        bool Any(Guid buildingId, Guid teamId);

        /// <summary>
        /// Removes an existing mapping between a building and a team.
        /// </summary>
        /// <param name="mapping">The <see cref="BuildingTeamMapping"/> object to be removed.</param>
        /// <returns>
        /// A boolean indicating whether the removal was successful.
        /// Returns false if the mapping does not exist or the operation failed.
        /// </returns>
        Task<bool> RemoveAsync(BuildingTeamMapping mapping);
    }
}
