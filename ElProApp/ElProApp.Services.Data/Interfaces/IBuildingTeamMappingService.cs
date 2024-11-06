namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IBuildingTeamMappingService
    {
        public Task<BuildingTeamMapping> AddAsync(Guid buildingId, Guid teamId);
        public Task<ICollection<BuildingTeamMapping>> GetAllByTeamId(Guid teamId);
    }
}
