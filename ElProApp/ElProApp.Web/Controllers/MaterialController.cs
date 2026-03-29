namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Web.Models.Material;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using static Common.ApplicationConstants;

    /// <summary>
    /// Manages material operations.
    /// </summary>
    [Authorize]
    public class MaterialController(IMaterialService materialService) : Controller
    {
        /// <summary>
        /// Displays all material mappings (materials per building).
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
        /// Displays material details (all buildings).
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
        [HttpGet]
        public async Task<IActionResult> AddToBuilding()
        {
            var model = new BuildingMaterialInputModel
            {
                Buildings = (await materialService.GetAddModelAsync()).Buildings,

                Materials = (await materialService.GetAllAsync())
                    .GroupBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Key.ToString(),
                        Text = x.First().Name
                    })
            };

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

            var mappingService =
                HttpContext.RequestServices.GetRequiredService<IBuildingMaterialMappingService>();

            await mappingService.IncreaseAsync(
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
        public async Task<IActionResult> SetPrice()
        {
            var model = new BuildingMaterialPriceInputModel
            {
                Buildings = (await materialService.GetAddModelAsync()).Buildings,

                Materials = (await materialService.GetAllAsync())
                    .GroupBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Key.ToString(),
                        Text = x.First().Name
                    })
            };

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

            var priceService =
                HttpContext.RequestServices.GetRequiredService<IBuildingMaterialPriceService>();

            await priceService.SetPriceAsync(
                model.BuildingId,
                model.MaterialId,
                model.Price);

            return RedirectToAction(nameof(All));
        }
    }
}
