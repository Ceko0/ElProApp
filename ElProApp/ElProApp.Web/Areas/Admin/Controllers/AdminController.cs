namespace ElProApp.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    using Models;
    using Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Web.Models.Job;
    using ElProApp.Web.Models.Team;
    using ElProApp.Services.Data;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController(IServiceProvider _serviceProvider,
            UserManager<IdentityUser> _userManager,
            RoleManager<IdentityRole> _roleManager)
            : Controller
    {
        private readonly IServiceProvider serviceProvider = _serviceProvider;
        private readonly UserManager<IdentityUser> userManager = _userManager;
        private readonly RoleManager<IdentityRole> roleManager = _roleManager;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUserRoles()
        {
            var users = userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            var rolesList = roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.AllRoles = rolesList;

            return View(userRolesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(string userId, List<string> roles, string state)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("Потребителят не е намерен.");
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            switch (state)
            {
                case "add":
                    var rolesToAdd = roles.Except(currentRoles).ToList();
                    await userManager.AddToRolesAsync(user, roles);
                    break;
                case "remove":
                    var rolesToRemove = currentRoles.Except(roles).ToList();
                    await userManager.RemoveFromRolesAsync(user, roles);
                    break;
            }

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

