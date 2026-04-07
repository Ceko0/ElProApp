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
        private readonly IBuildingMaterialMappingService buildingMaterialService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialService"/> class.
        /// </summary>
        public MaterialService(
            IRepository<Material, Guid> materialRepository,
            IHelpMethodsService helpMethodsService,
            IBuildingMaterialMappingService buildingMaterialService)
        {
            this.materialRepository = materialRepository;
            this.helpMethodsService = helpMethodsService;
            this.buildingMaterialService = buildingMaterialService;
        }

        /// <summary>
        /// Prepares a material creation input model.
        /// </summary>
        public async Task<MaterialInputModel> GetAddModelAsync()
        {
            return new MaterialInputModel
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
        }

        /// <summary>
        /// Creates a new material and assigns initial quantity to a building.
        /// </summary>
        public async Task<string> AddAsync(MaterialInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            bool exists = await materialRepository
                .GetAllAttached()
                .AnyAsync(x => x.Name == model.Name && !x.IsDeleted);

            if (exists)
                throw new InvalidOperationException(
                    "Material with the same name already exists.");

            var entity =
                AutoMapperConfig.MapperInstance.Map<Material>(model);

            await materialRepository.AddAsync(entity);

            if (model.BuildingId != Guid.Empty && model.Quantity > 0)
            {
                await buildingMaterialService.IncreaseAsync(
                    model.BuildingId,
                    entity.Id,
                    model.Quantity);
            }

            return entity.Id.ToString();
        }

        /// <summary>
        /// Retrieves a material edit model by identifier.
        /// </summary>
        public async Task<MaterialEditInputModel> GetEditByIdAsync(string id)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await materialRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(x => x.Id == validId && !x.IsDeleted)
                ?? throw new InvalidOperationException("Material not found or is deleted.");

            return new MaterialEditInputModel
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Updates an existing material.
        /// </summary>
        public async Task<bool> EditByModelAsync(MaterialEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await materialRepository
                .GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("Material not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("Material is deleted.");

            entity.Name = model.Name;

            await materialRepository.UpdateAsync(entity);

            return true;
        }

        /// <summary>
        /// Retrieves all materials.
        /// </summary>
        public async Task<ICollection<MaterialViewModel>> GetAllAsync()
        {
            var materials = await materialRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            return materials
                .Select(m => new MaterialViewModel
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToList();
        }

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

            var entity = await materialRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(x => x.Id == validId && !x.IsDeleted)
                ?? throw new InvalidOperationException("Material not found or is deleted.");

            var buildingMaterials = await buildingMaterialService
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => x.MaterialId == validId)
                .ToListAsync();

            return new MaterialViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingMaterials = buildingMaterials
                    .Select(x => new BuildingMaterialViewModel
                    {
                        MaterialId = x.MaterialId,
                        MaterialName = entity.Name,
                        BuildingId = x.BuildingId,
                        BuildingName = x.Building.Name,
                        Quantity = x.Quantity
                    })
                    .ToList()
            };
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
        public async Task<ICollection<BuildingMaterialViewModel>> GetByBuildingIdAsync(string buildingId)
        {
            Guid validId =
                helpMethodsService.ConvertAndTestIdToGuid(buildingId);

            return await buildingMaterialService
                .GetAllAttached()
                .Include(x => x.Material)
                .Include(x => x.Building)
                .Where(x => x.BuildingId == validId)
                .Select(x => new BuildingMaterialViewModel
                {
                    MaterialId = x.MaterialId,
                    MaterialName = x.Material.Name,
                    BuildingId = x.BuildingId,
                    BuildingName = x.Building.Name,
                    Quantity = x.Quantity
                })
                .ToListAsync();
        }
    }
}