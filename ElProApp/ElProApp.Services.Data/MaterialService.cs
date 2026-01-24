namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.Material;

    public class MaterialService : IMaterialService
    {
        private readonly IRepository<Material, Guid> materialRepository;
        private readonly IHelpMethodsService helpMethodsService;

        public MaterialService(
            IRepository<Material, Guid> materialRepository,
            IHelpMethodsService helpMethodsService)
        {
            this.materialRepository = materialRepository;
            this.helpMethodsService = helpMethodsService;
        }

        public async Task<MaterialInputModel> GetAddModelAsync()
        {
            var model = new MaterialInputModel();
            model.Buildings = await helpMethodsService
                .GetAllBuildings()
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();
            return model;
        }

        public async Task<string> AddAsync(MaterialInputModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingMaterial = await materialRepository
                .FirstOrDefaultAsync(x => x.Name == model.Name && !x.IsDeleted);

                var entity = AutoMapperConfig.MapperInstance.Map<Material>(model);
                await materialRepository.AddAsync(entity);
            

            await materialRepository.SaveAsync();
            return entity.Id.ToString();
        }

        public async Task<MaterialEditInputModel> GetEditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var model = await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<MaterialEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (model == null)
                throw new InvalidOperationException("Material not found or is deleted.");

            model.Buildings = await helpMethodsService
                .GetAllBuildings()
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name,
                    Selected =b.Id == model.BuildingId
                    })
                .ToListAsync();
            return model;
        }

        public async Task<bool> EditByModelAsync(MaterialEditInputModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var entity = await materialRepository.GetByIdAsync(model.Id);
            if (entity == null || entity.IsDeleted)
                return false;

            AutoMapperConfig.MapperInstance.Map(model, entity);
            await materialRepository.SaveAsync();

            return true;
        }

        public async Task<ICollection<MaterialViewModel>> GetAllAsync()
        {
            return await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .Select(x => new MaterialViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public IQueryable<Material> GetAllAttached()
            => materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        public async Task<MaterialViewModel?> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var model = await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<MaterialViewModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (model == null)
                throw new InvalidOperationException("Material not found or is deleted.");

            return model;
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            return await materialRepository.SoftDeleteAsync(validId);
        }

        public async Task<ICollection<BuildingMaterialViewModel>> GetByBuildingIdAsync(string buildingId)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(buildingId);

            return await materialRepository
                .GetAllAttached()
                .SelectMany(m => m.Buildings)
                .Where(bm => bm.BuildingId == validId)
                .Select(bm => new BuildingMaterialViewModel
                {
                    MaterialId = bm.MaterialId,
                    MaterialName = bm.Material.Name,
                    BuildingId = bm.BuildingId,
                    BuildingName = bm.Building.Name,
                    Quantity = bm.Quantity
                })
                .ToListAsync();
        }

    }
}
