namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;
    using Mapping;
    using Web.Models.Building;

    public class BuildingService(IRepository<Building, Guid> buildingRepository,
                                 IBuildingTeamMappingService buildingTeamMappingService,
                                 IHelpMethodsService helpMethodsService):
                                 IBuildingService
    {
        /// <summary>
        /// Adds a new building based on the provided input model.
        /// </summary>
        /// <param name="model">The input model containing building details.</param>
        /// <returns>The ID of the newly added building.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a building with the same name already exists.</exception>
        public async Task<string> AddAsync(BuildingInputModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Building model cannot be null.");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Building name must be provided.");
            }

            var existingBuilding = await buildingRepository.FirstOrDefaultAsync(x => x.Name == model.Name);
            if (existingBuilding != null)
                throw new InvalidOperationException("A building with this name already exists!");

            var building = AutoMapperConfig.MapperInstance.Map<Building>(model);
            await buildingRepository.AddAsync(building);
            return building.Id.ToString();
        }


        /// <summary>
        /// Retrieves the edit input model for a building by its ID.
        /// </summary>
        /// <param name="id">The ID of the building.</param>
        /// <returns>The edit input model for the specified building.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the building is deleted.</exception>
        public async Task<BuildingEditInputModel> GetEditByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Building ID must be provided.");
            }

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var model = await buildingRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<BuildingEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (model == null)
            {
                throw new InvalidOperationException("Building not found or is deleted.");
            }

            model.TeamsOnBuilding = await helpMethodsService.GetBuildingTeamMapping(model.Id);
            return model;
        }

        /// <summary>
        /// Updates a building based on the provided edit input model.
        /// </summary>
        /// <param name="model">The input model containing updated building details.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> EditByModelAsync(BuildingEditInputModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Building edit model cannot be null.");
            }

            var entity = await buildingRepository.GetByIdAsync(model.Id);
            if (entity == null)
            {
                throw new InvalidOperationException("Building not found.");
            }

            AutoMapperConfig.MapperInstance.Map(model, entity);
            await buildingRepository.SaveAsync();

            var buildingTeamMappings = await buildingTeamMappingService
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.Building)
                .Where(m => m.BuildingId == model.Id && !m.Team.IsDeleted && !m.Building.IsDeleted)
                .ToListAsync();

            foreach (var buildingTeamMapping in buildingTeamMappings)
            {
                if (!model.selectedTeamEntities.Contains(buildingTeamMapping.TeamId))
                {
                    var mappingToRemove = buildingTeamMappings.FirstOrDefault(m => m.TeamId == buildingTeamMapping.TeamId);
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

        /// <summary>
        /// Retrieves a list of all buildings.
        /// </summary>
        /// <returns>A collection of view models representing all buildings.</returns>
        public async Task<ICollection<BuildingViewModel>> GetAllAsync()
        {
            var model = await buildingRepository.GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<BuildingViewModel>()
                .ToListAsync();

            foreach (var building in model)
            {
                building.TeamsOnBuilding = await helpMethodsService.GetBuildingTeamMapping(building.Id);
            }

            return model;
        }

        /// <summary>
        /// Retrieves all attached buildings.
        /// </summary>
        /// <returns>An <see cref="IQueryable{Building}"/> collection of all attached buildings.</returns>
        public IQueryable<Building> GetAllAttached()
            => buildingRepository.GetAllAttached().Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a building by its ID.
        /// </summary>
        /// <param name="id">The ID of the building.</param>
        /// <returns>The view model representing the building.</returns>
        public async Task<BuildingViewModel> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var model = buildingRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<BuildingViewModel>()
                .FirstOrDefault(x => x.Id == validId);
            if (model == null) throw new InvalidOperationException("Building not found or is deleted.");

            model.TeamsOnBuilding = await helpMethodsService.GetBuildingTeamMapping(model.Id);

            return model!;
        }

        /// <summary>
        /// Soft deletes a building by its ID.
        /// </summary>
        /// <param name="id">The ID of the building to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Building ID must be provided.");
            }

            try
            {
                Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
                bool isDeleted = await buildingRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
