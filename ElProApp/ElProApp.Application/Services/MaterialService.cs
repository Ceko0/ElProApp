namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Material;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides application-level operations for managing materials.
    /// </summary>
    public class MaterialService : IMaterialService
    {
        private readonly IRepository<Material, Guid> materialRepository;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialService"/> class.
        /// </summary>
        public MaterialService(
            IRepository<Material, Guid> materialRepository,
            IHelpMethodsService helpMethodsService)
        {
            this.materialRepository = materialRepository;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Prepares a material creation input model.
        /// </summary>
        public async Task<MaterialInputModel> GetAddModelAsync()
        {
            var model = new MaterialInputModel
            {
                Buildings = await helpMethodsService
                    .GetAllBuildings()
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    })
                    .ToListAsync()
            };

            return model;
        }

        /// <summary>
        /// Creates a new material.
        /// </summary>
        public async Task<string> AddAsync(MaterialInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            bool exists = await materialRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.Name == model.Name &&
                    !x.IsDeleted);

            if (exists)
                throw new InvalidOperationException(
                    "Material with the same name already exists.");

            var entity =
                AutoMapperConfig.MapperInstance.Map<Material>(model);

            await materialRepository.AddAsync(entity);
            await materialRepository.SaveAsync();

            return entity.Id.ToString();
        }

        /// <summary>
        /// Retrieves a material edit model by identifier.
        /// </summary>
        public async Task<MaterialEditInputModel> GetEditByIdAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            var model = await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<MaterialEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException(
                    "Material not found or is deleted.");

            model.Buildings = await helpMethodsService
                .GetAllBuildings()
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name,
                    Selected = b.Id == model.BuildingId
                })
                .ToListAsync();

            return model;
        }

        /// <summary>
        /// Updates an existing material.
        /// </summary>
        public async Task<bool> EditByModelAsync(MaterialEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await materialRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException(
                    "Material not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Material is deleted.");

            AutoMapperConfig.MapperInstance.Map(model, entity);
            await materialRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// Retrieves all non-deleted materials.
        /// </summary>
        public async Task<ICollection<MaterialViewModel>> GetAllAsync()
            => await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .Select(x => new MaterialViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

        /// <summary>
        /// Returns all attached, non-deleted materials.
        /// </summary>
        public IQueryable<Material> GetAllAttached()
            => materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a material by identifier.
        /// </summary>
        public async Task<MaterialViewModel> GetByIdAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            var model = await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<MaterialViewModel>()
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException(
                    "Material not found or is deleted.");

            return model;
        }

        /// <summary>
        /// Soft deletes a material by identifier.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            return await materialRepository
                .SoftDeleteAsync(validId);
        }

        /// <summary>
        /// Retrieves all materials assigned to a specific building.
        /// </summary>
        public async Task<ICollection<BuildingMaterialViewModel>> GetByBuildingIdAsync(
            string buildingId)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(buildingId);

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
