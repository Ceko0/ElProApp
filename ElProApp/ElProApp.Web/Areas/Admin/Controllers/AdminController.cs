namespace ElProApp.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Data.Models;
    using ElProApp.Web.Infrastructure.Interfaces;

    [Area("Admin")]
    [Authorize(Roles = "Admin,OfficeManager,Technician")]
    public class AdminController(IServiceProvider serviceProvider,
            IAdminService adminService)
            : Controller
    {
        [Authorize(Roles = "Admin,OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> UpdateUserRoles()
        {
            var model = await adminService.GetUsersRolesAsync();
            ViewBag.AllRoles = await adminService.GetAllRolesAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin,OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> roles, string state)
        {
            var isSuccess = await adminService.PostUsersRolesAsync(userId, roles, state);

            if (!isSuccess)
                return RedirectToAction("Index", "Home");

            return RedirectToAction(nameof(UpdateUserRoles));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeletedEntitiesAsync(string entityType)
        {
            if (string.IsNullOrEmpty(entityType))
                return BadRequest("Entity type is required.");

            object deletedEntities;

            switch (entityType)
            {
                case "Employee":
                    deletedEntities = await adminService.GetDeletedEntitiesAsync<Employee>();
                    break;
                case "Building":
                    deletedEntities = await adminService.GetDeletedEntitiesAsync<Building>();
                    break;
                case "Team":
                    deletedEntities = await adminService.GetDeletedEntitiesAsync<Team>();
                    break;                
                case "JobDone":
                    deletedEntities = await adminService.GetDeletedEntitiesAsync<JobDone>();
                    break;
                case "Material":
                    deletedEntities = await adminService.GetDeletedEntitiesAsync<Material>();
                    break;
                default:
                    return BadRequest("Invalid entity type.");
            }

            if (deletedEntities == null)
                return NotFound("No deleted entities found.");

            return View(deletedEntities);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeletedEntities(string id, string entityType)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id is missing.");

            if (string.IsNullOrEmpty(entityType))
                return BadRequest("Entity type is required.");

            bool result = false;

            switch (entityType)
            {
                case "Employee":
                    result = await adminService.RestoreDeletedEntityAsync<Employee>(id);
                    break;
                case "Building":
                    result = await adminService.RestoreDeletedEntityAsync<Building>(id);
                    break;
                case "Team":
                    result = await adminService.RestoreDeletedEntityAsync<Team>(id);
                    break;               
                case "JobDone":
                    result = await adminService.RestoreDeletedEntityAsync<JobDone>(id);
                    break;
                case "Material":
                    result = await adminService.RestoreDeletedEntityAsync<Material>(id);
                    break;
                default:
                    return BadRequest("Invalid entity type.");
            }

            if (!result)
                return NotFound("Entity not found.");

            return RedirectToAction(nameof(DeletedEntities), new { entityType });
        }

    }
}