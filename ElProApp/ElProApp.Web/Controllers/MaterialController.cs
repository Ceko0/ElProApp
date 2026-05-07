namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Web.Models.Material;

    using static Common.ApplicationConstants;

    /// <summary>
    /// Manages material operations.
    /// </summary>
    [Authorize]
    public class MaterialController : Controller
    {
        private readonly IMaterialService materialService;
        private readonly IBuildingMaterialMappingService buildingMaterialMappingService;
        private readonly IBuildingMaterialPriceService buildingMaterialPriceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialController"/> class.
        /// </summary>
        public MaterialController(
            IMaterialService materialService,
            IBuildingMaterialMappingService buildingMaterialMappingService,
            IBuildingMaterialPriceService buildingMaterialPriceService)
        {
            this.materialService = materialService;
            this.buildingMaterialMappingService = buildingMaterialMappingService;
            this.buildingMaterialPriceService = buildingMaterialPriceService;
        }

        /// <summary>
        /// Displays all materials.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> All()
            => View(await materialService.GetAllAsync());

        /// <summary>
        /// Displays add material form.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Add()
            => View(await materialService.GetAddModelAsync());

        /// <summary>
        /// Creates a new material.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> Add(MaterialInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = await materialService.AddAsync(model);
            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Displays material details.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName},{TechnicianRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadHttpRequestException("Material ID cannot be empty.");

            var model = await materialService.GetByIdAsync(id);
            return View(model);
        }

        /// <summary>
        /// Displays edit form (material name only).
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await materialService.GetEditByIdAsync(id);
            return View(model);
        }

        /// <summary>
        /// Edits material (name only).
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> Edit(MaterialEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await materialService.EditByModelAsync(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        /// <summary>
        /// Soft deletes a material.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            await materialService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(All));
        }

        /// <summary>
        /// Displays materials for a specific building.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ByBuilding(string buildingId)
        {
            var materials = await materialService.GetByBuildingIdAsync(buildingId);
            return View(materials);
        }

        /// <summary>
        /// Displays form for assigning material to a building.
        /// </summary>
        /// <param name="buildingId">Optional building identifier.</param>
        /// <param name="materialId">Optional material identifier.</param>
        [HttpGet]
        public async Task<IActionResult> AddToBuilding(Guid? buildingId, Guid? materialId)
        {
            var model = new BuildingMaterialInputModel
            {
                Buildings = (await materialService.GetAddModelAsync()).Buildings,

                Materials = (await materialService.GetAllAsync())
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
            };

            if (buildingId.HasValue)
                model.BuildingId = buildingId.Value;

            if (materialId.HasValue)
                model.MaterialId = materialId.Value;

            return View(model);
        }

        /// <summary>
        /// Assigns material quantity to a building.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToBuilding(BuildingMaterialInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await buildingMaterialMappingService.IncreaseAsync(
                model.BuildingId,
                model.MaterialId,
                model.Quantity);

            return RedirectToAction(nameof(All));
        }

        /// <summary>
        /// Displays form for setting material price per building.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpGet]
        public async Task<IActionResult> SetPrice(Guid? buildingId, Guid? materialId)
        {
            var model = new BuildingMaterialPriceInputModel
            {
                Buildings = (await materialService.GetAddModelAsync()).Buildings,

                Materials = (await materialService.GetAllAsync())
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
            };

            if (buildingId.HasValue)
                model.BuildingId = buildingId.Value;

            if (materialId.HasValue)
                model.MaterialId = materialId.Value;

            return View(model);
        }

        /// <summary>
        /// Sets or updates material price for a building.
        /// </summary>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> SetPrice(BuildingMaterialPriceInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await buildingMaterialPriceService.SetPriceAsync(
                model.BuildingId,
                model.MaterialId,
                model.Price);

            return RedirectToAction(nameof(All));
        }

        /// <summary>
        /// Displays all prices for a given material across buildings.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <returns>
        /// A view containing all building-price mappings for the specified material.
        /// </returns>
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Prices(Guid materialId)
        {
            var priceService =
                HttpContext.RequestServices.GetRequiredService<IBuildingMaterialPriceService>();

            var model = await priceService.GetAllByMaterialIdAsync(materialId);

            return View(model);
        }
        /// <summary>
        /// Displays edit quantities for materials in a building.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName},{TechnicianRoleName}")]
        public async Task<IActionResult> EditQuantities(string buildingId)
        {
            if (string.IsNullOrWhiteSpace(buildingId))
                throw new BadHttpRequestException("Building ID is required.");

            var materials = await materialService.GetByBuildingIdAsync(buildingId);

            return View(materials);
        }

        /// <summary>
        /// Updates quantities for materials in a building.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AdminRoleName},{OfficeManagerRoleName},{TechnicianRoleName}")]
        public async Task<IActionResult> EditQuantities(string buildingId, List<BuildingMaterialViewModel> model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var mappingService =
                HttpContext.RequestServices.GetRequiredService<IBuildingMaterialMappingService>();

            foreach (var item in model)
            {
                await mappingService.SetQuantityAsync(
                    item.BuildingId,
                    item.MaterialId,
                    item.Quantity);
            }

            return RedirectToAction("Details", "Building", new { id = buildingId });
        }
    }
}