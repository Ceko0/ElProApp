namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Web.Models.Material;

    [Authorize]
    public class MaterialController (IMaterialService materialService): Controller
    {

        [HttpGet]
        public async Task<IActionResult> All() =>
            View(await materialService.GetAllAsync());
        

        [Authorize(Roles = "Admin,OfficeManager,Technician")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(await materialService.GetAddModelAsync());
        }

        [Authorize(Roles = "Admin,OfficeManager,Technician")]
        [HttpPost]
        public async Task<IActionResult> Add(MaterialInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Buildings = (await materialService.GetAddModelAsync()).Buildings;
                return View(model);
            }

            var id = await materialService.AddAsync(model);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin,OfficeManager,Technician,Worker")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            return View(await materialService.GetByIdAsync(id));
        }

        [Authorize(Roles = "Admin,OfficeManager,Technician")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return View(await materialService.GetEditByIdAsync(id));
        }

        [Authorize(Roles = "Admin,OfficeManager,Technician")]
        [HttpPost]
        public async Task<IActionResult> Edit(MaterialEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Buildings = (await materialService.GetAddModelAsync()).Buildings;
                return View(model);
            }

            await materialService.EditByModelAsync(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = "Admin,OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            await materialService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(All));
        }
    }
}
