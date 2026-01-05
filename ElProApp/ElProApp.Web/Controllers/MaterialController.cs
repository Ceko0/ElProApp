namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using Models.Material;
    using static Common.ApplicationConstants;

    [Authorize]
    public class MaterialController(IMaterialService materialService ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (User.IsInRole(AdminRoleName) || User.IsInRole(OfficeManagerRoleName) || User.IsInRole(TechnicianRoleName))
                return RedirectToAction("AllMaterials", "Admin", new { area = "admin" });

            return View(await materialService.GetAllAsync());
        }

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = await materialService.GetAddModelAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpPost]
        public async Task<IActionResult> Add(MaterialInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                string Id = await materialService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = Id });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
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

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
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

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
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

        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var idCheck = string.IsNullOrWhiteSpace(id);
            if (idCheck) throw new BadHttpRequestException("Material ID cannot be empty.");

            var employee = await materialService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Material not found.");
            }

            bool isDeleted = await materialService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
