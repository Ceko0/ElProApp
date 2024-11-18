namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using Microsoft.Extensions.DependencyInjection;
    using ElProApp.Data.Models.Mappings;

    internal class BuildingService(IRepository<Building, Guid> _BuildingRepository
        , IBuildingTeamMappingService _buildingTeamMappingService
        , IServiceProvider _serviceProvider)
        : IBuildingService
    {
        private readonly IRepository<Building, Guid> buildingRepository = _BuildingRepository;
        private readonly IBuildingTeamMappingService buildingTeamMappingService = _buildingTeamMappingService;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        public async Task<string> AddAsync(BuildingInputModel model)
        {
            if ((await buildingRepository.FirstOrDefaultAsync(x => x.Name == model.Name)) != null)
                throw new InvalidOperationException("A building with this name already exists!");
            var building = AutoMapperConfig.MapperInstance.Map<Building>(model);

            await buildingRepository.AddAsync(building);
            return building.Id.ToString();
        }

        public async Task<BuildingEditInputModel> GetEditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var model = await buildingRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<BuildingEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (model == null) throw new InvalidOperationException("Team is deleted.");

            model.TeamsOnBuilding = await GetBuildingTeamMapping(model.Id);

            return model;
        }

        public async Task<bool> EditByModelAsync(BuildingEditInputModel model)
        {
            try
            {
                var entity = await buildingRepository.GetByIdAsync(model.Id);
                AutoMapperConfig.MapperInstance.Map(model, entity);
                await buildingRepository.SaveAsync();

                var BuildingTeamMappingService = _serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

                var BuildingTeamMappings = await buildingTeamMappingService.GetAllAttached().Include( x => x.Team).Where(m => m.BuildingId == model.Id).ToListAsync();
                foreach (var buildingTeamMapping in BuildingTeamMappings)
                {
                    if (!model.selectedTeamEntities.Contains(buildingTeamMapping.TeamId))
                    {
                        var mappingToRemove = BuildingTeamMappings.FirstOrDefault(m => m.TeamId == buildingTeamMapping.TeamId);
                        if (mappingToRemove != null)
                        {
                            await buildingTeamMappingService.RemoveAsync(mappingToRemove);
                        }
                    }
                }

                foreach (var teamId in model.selectedTeamEntities)
                {
                    if (!buildingTeamMappingService.Any(model.Id, teamId))
                        await buildingTeamMappingService.AddAsync(model.Id, teamId);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ICollection<BuildingViewModel>> GetAllAsync()
        {
            var model = await buildingRepository.GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<BuildingViewModel>()
                .ToListAsync();

            foreach (var building in model)
            {
                building.TeamsOnBuilding = await GetBuildingTeamMapping(building.Id);
            }

            return model;
        }

        public IQueryable<Building> GetAllAttached()
            => buildingRepository.GetAllAttached();

        public async Task<BuildingViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var model = buildingRepository
                .GetAllAttached()
                .To<BuildingViewModel>()
                .FirstOrDefault(x => x.Id == validId);

            model.TeamsOnBuilding = await GetBuildingTeamMapping(model.Id);

            return model!;
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await buildingRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch
            {
                return false;
            }
        }


        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        private async Task<List<BuildingTeamMapping>> GetBuildingTeamMapping(Guid id) => await buildingTeamMappingService
                                .GetAllAttached()
                                .Include(x => x.Building)
                                .Include(x => x.Team)
                                .Where(x => x.BuildingId == id).ToListAsync();

    }
}
