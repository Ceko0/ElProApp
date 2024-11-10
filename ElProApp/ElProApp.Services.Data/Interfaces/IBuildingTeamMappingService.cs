namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IBuildingTeamMappingService
    {
        public Task<BuildingTeamMapping> AddAsync(Guid buildingId, Guid teamId);
        public Task<ICollection<BuildingTeamMapping>> GetByTeamIdAsync(Guid teamId);
        public Task<ICollection<BuildingTeamMapping>> GetByBuildingIdAsync(Guid teamId);
        public Task<ICollection<BuildingTeamMapping>> GetAllAttachedAsync();
        IQueryable<BuildingTeamMapping> GetAllAttached();
        public bool Any(Guid buildingId, Guid teamId);
        public Task<bool> RemoveAsync(BuildingTeamMapping mapping);
    }
}
