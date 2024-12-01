namespace ElProApp.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ElProApp.Web.Models.Admin;
    using Services.Data.Interfaces;

    [Area("Admin")]
    [Authorize(Roles = "Admin , OfficeManager")]
    public class AdminController(IServiceProvider _serviceProvider,
            IAdminService _adminService)
            : Controller
    {
        private readonly IServiceProvider serviceProvider = _serviceProvider;
        private readonly IAdminService adminService = _adminService;
                
        [HttpGet]
        public async Task<IActionResult> UpdateUserRoles()
        {
            var model = await adminService.GetUsersRolesAsync();

            ViewBag.AllRoles = await adminService.GetAllRolesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> roles, string state)
        {
            var isSuccess = await adminService.PostUsersRolesAsync(userId,roles, state);

            if (!isSuccess) return RedirectToAction("Index", "Home");

            return RedirectToAction(nameof(UpdateUserRoles));
        }

        [HttpGet]
        public async Task<IActionResult> AllBuildings()
        {
            var Service = serviceProvider.GetRequiredService<IBuildingService>();

            return View(await Service.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AllEmployees()
        {
            var Service = serviceProvider.GetRequiredService<IEmployeeService>();

            return View(await Service.GetAllAsync()); ;
        }

        [HttpGet]
        public async Task<IActionResult> AllJobDones()
        {
            var Service = serviceProvider.GetRequiredService<IJobDoneService>();

            return View(await Service.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AllJobs()
        {
            var Service = serviceProvider.GetRequiredService<IJobService>();

            return View(await Service.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AllTeams()
        {
            var Service = serviceProvider.GetRequiredService<ITeamService>();

            return View(await Service.GetAllAsync());
        }
    }

}

