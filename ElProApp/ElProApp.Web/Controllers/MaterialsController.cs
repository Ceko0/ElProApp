namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.Material;
    using static Common.ApplicationConstants;

    [Authorize]
    public class MaterialsController(IMaterialService materialService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> All()
        {
            try
            {
                return View(await materialService.GetAllAsync());
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName}")]
        [HttpGet]
        public IActionResult Add() => View();

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> Add(MaterialInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string materialId = await materialService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = materialId });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName} , {TechnicianRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadHttpRequestException("Material ID cannot be empty.");

            try
            {
                var model = await materialService.GetByIdAsync(id);
                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadHttpRequestException("Material ID cannot be empty.");

            try
            {
                var model = await materialService.GetEditByIdAsync(id);
                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> Edit(MaterialEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isEdited = await materialService.EditByModelAsync(model);
            if (!isEdited)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = $"{AdminRoleName} , {OfficeManagerRoleName}")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadHttpRequestException("Material ID cannot be empty.");

            try
            {
                bool isDeleted = await materialService.SoftDeleteAsync(id);
                if (isDeleted)
                {
                    return RedirectToAction(nameof(All));
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }
    }
}
