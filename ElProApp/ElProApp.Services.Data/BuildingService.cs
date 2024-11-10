namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;


    internal class BuildingService(IRepository<Building , Guid> _BuildingRepository) : IBuildingService
    {
        private readonly IRepository<Building, Guid> buildingRepository = _BuildingRepository;

        public async Task<string> AddAsync(BuildingInputModel model)
        {
            if ((await buildingRepository.FirstOrDefaultAsync(x => x.Name == model.Name)) != null)
                     throw new InvalidOperationException("A building with this name already exists!");
            var building = AutoMapperConfig.MapperInstance.Map<Building>(model);

            await buildingRepository.AddAsync(building);
            return building.Id.ToString();
        }

        public async Task<BuildingEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await buildingRepository.GetByIdAsync(validId);
            if (entity.IsDeleted) throw new InvalidOperationException("Team is deleted.");

            return AutoMapperConfig.MapperInstance.Map<BuildingEditInputModel>(entity);
        }

        public async Task<bool> EditByModelAsync(BuildingEditInputModel model)
        {
            try
            {
                var entity = await buildingRepository.GetByIdAsync(model.Id);
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await buildingRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ICollection<BuildingViewModel>> GetAllAsync()
        {
            var building = await buildingRepository.GetAllAttached()
                                   .Include(x => x.TeamsOnBuilding)
                                   .ThenInclude(tb => tb.Team)
                                   .Where(x => !x.IsDeleted)
                                   .To <BuildingViewModel>()
                                   .ToListAsync();
            return (building);
        }

        public async Task<BuildingViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await buildingRepository.GetByIdAsync(validId).ConfigureAwait(false);
            return entity != null
                ? AutoMapperConfig.MapperInstance.Map<BuildingViewModel>(entity)
                : throw new ArgumentException("Missing entity.");
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
