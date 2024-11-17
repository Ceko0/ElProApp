using System.Diagnostics;

namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using Microsoft.Extensions.DependencyInjection;

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
                .Include(x => x.TeamsOnBuilding)
                .ThenInclude( t => t.Team)
                .To<BuildingEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);
                       
            if (model == null) throw new InvalidOperationException("Team is deleted.");

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

                var existingTeamIds = await buildingTeamMappingService.GetAllAttachedAsync();
                foreach (var buildingTeamMapping in existingTeamIds)
                {
                    if (!model.selectedTeamEntities.Contains(buildingTeamMapping.TeamId))
                    {
                        var mappingToRemove = entity.TeamsOnBuilding.FirstOrDefault(m => m.TeamId == buildingTeamMapping.TeamId);
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
            => await buildingRepository.GetAllAttached()
                .Include(x => x.TeamsOnBuilding)
                .ThenInclude(tb => tb.Team)
                .Where(x => !x.IsDeleted)
                .To<BuildingViewModel>()
                .ToListAsync();

        public IQueryable<Building> GetAllAttached()
            => buildingRepository.GetAllAttached();

        public BuildingViewModel GetById(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var model = buildingRepository
                .GetAllAttached()
                .Include(x => x.TeamsOnBuilding)
                .ThenInclude(tb => tb.Team)
                .To<BuildingViewModel>()
                .FirstOrDefault(x => x.Id == validId);

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
    }
}
