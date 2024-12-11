namespace ElProApp.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services.Data.Interfaces;

    using ElProApp.Data.Models;

    [Area("Admin")]
    [Authorize(Roles = "Admin , OfficeManager , Technician")]
    public class AdminController(IServiceProvider serviceProvider,
            IAdminService adminService)
            : Controller
    {
        /// <summary>
        /// Retrieves the list of users with their roles and available roles for updating.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with the user roles data.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> UpdateUserRoles()
        {
            var model = await adminService.GetUsersRolesAsync();

            ViewBag.AllRoles = await adminService.GetAllRolesAsync();

            return View(model);
        }

        /// <summary>
        /// Updates the roles of a user based on the provided user ID, roles, and action state (add/remove).
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose roles are being updated.</param>
        /// <param name="roles">The list of roles to be added or removed.</param>
        /// <param name="state">The action to perform on the roles ("add" or "remove").</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the redirection to the UpdateUserRoles view or Home page if unsuccessful.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> roles, string state)
        {
            var isSuccess = await adminService.PostUsersRolesAsync(userId, roles, state);

            if (!isSuccess) return RedirectToAction("Index", "Home");

            return RedirectToAction(nameof(UpdateUserRoles));
        }

        /// <summary>
        /// Retrieves and displays deleted entities of the specified type.
        /// </summary>
        /// <param name="entityType">The type of entities to retrieve (e.g., Employee, Building, etc.).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with the deleted entities or a bad request if the entity type is invalid.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeletedEntities(string entityType)
        {
            if (string.IsNullOrEmpty(entityType))
            {
                return BadRequest("Entity type is required.");
            }

            object deletedEntities;

            switch (entityType)
            {
                case "Employee":
                    deletedEntities = adminService.GetDeletedEntities<Employee>();
                    break;
                case "Building":
                    deletedEntities = adminService.GetDeletedEntities<Building>();
                    break;
                case "Team":
                    deletedEntities = adminService.GetDeletedEntities<Team>();
                    break;
                case "Job":
                    deletedEntities = adminService.GetDeletedEntities<Job>();
                    break;
                case "JobDone":
                    deletedEntities = adminService.GetDeletedEntities<JobDone>();
                    break;
                default:
                    return BadRequest("Invalid entity type.");
            }

            if (deletedEntities == null)
            {
                return NotFound("No deleted entities found.");
            }

            return View(deletedEntities);
        }

        /// <summary>
        /// Restores a deleted entity of the specified type based on the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to restore.</param>
        /// <param name="entityType">The type of the entity to restore (e.g., Employee, Building, etc.).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the redirection to the DeletedEntities view or a bad request if invalid parameters are provided.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeletedEntities(string id, string entityType)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Id is missing.");
            }

            if (string.IsNullOrEmpty(entityType))
            {
                return BadRequest("Entity type is required.");
            }

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
                case "Job":
                    result = await adminService.RestoreDeletedEntityAsync<Job>(id);
                    break;
                case "JobDone":
                    result = await adminService.RestoreDeletedEntityAsync<JobDone>(id);
                    break;
                default:
                    return BadRequest("Invalid entity type.");
            }

            if (!result)
            {
                return NotFound("Entity not found.");
            }

            return RedirectToAction("DeletedEntities", new { entityType = entityType });
        }

        /// <summary>
        /// Retrieves and displays all buildings.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with all buildings.</returns>
        [HttpGet]
        public async Task<IActionResult> AllBuildings()
        {
            var Service = serviceProvider.GetRequiredService<IBuildingService>();

            return View(await Service.GetAllAsync());
        }

        /// <summary>
        /// Retrieves and displays all employees.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with all employees.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> AllEmployees()
        {
            var Service = serviceProvider.GetRequiredService<IEmployeeService>();

            return View(await Service.GetAllAsync()); ;
        }

        /// <summary>
        /// Retrieves and displays all completed jobs (JobDone).
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with all completed jobs.</returns>
        [HttpGet]
        public async Task<IActionResult> AllJobDones()
        {
            var Service = serviceProvider.GetRequiredService<IJobDoneService>();

            return View(await Service.GetAllAsync());
        }

        /// <summary>
        /// Retrieves and displays all jobs.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with all jobs.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> AllJobs()
        {
            var Service = serviceProvider.GetRequiredService<IJobService>();

            return View(await Service.GetAllAsync());
        }

        /// <summary>
        /// Retrieves and displays all teams.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view with all teams.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> AllTeams()
        {
            var Service = serviceProvider.GetRequiredService<ITeamService>();

            return View(await Service.GetAllAsync());
        }
    }
}
