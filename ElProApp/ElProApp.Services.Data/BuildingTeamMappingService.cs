namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class BuildingTeamMappingService(IRepository<BuildingTeamMapping, Guid> _buildingTeamMappingRepository) : IBuildingTeamMappingService
    {
        private readonly IRepository<BuildingTeamMapping, Guid> buildingTeamMappingRepository = _buildingTeamMappingRepository;
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

        public async Task<ICollection<BuildingTeamMapping>> GetAllByTeamId(Guid teamId) => await buildingTeamMappingRepository.GetAllAttached().Where(x => x.TeamId == teamId).ToListAsync();
        
    }
}
