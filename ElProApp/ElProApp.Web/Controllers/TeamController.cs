namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.Team;

    [Authorize]
    public class TeamController(ITeamService _teamService, IBuildingService _buildingService, IJobDoneService _JobDoneService , IEmployeeService _employeeService) : Controller
    {
        private readonly ITeamService teamService = _teamService;
        private readonly IEmployeeService employeeService = _employeeService;
        private readonly IJobDoneService jobDoneServices = _JobDoneService;
        private readonly IBuildingService buildingService = _buildingService;

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var teams = await teamService.GetAllAsync();
            return View(teams);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid team ID.");
            }

            try
            {
                var team = await teamService.GetByIdAsync(id);
                return View(team);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
         
           var model = await teamService.AddAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TeamInputModel model)
        {
            if (!ModelState.IsValid) return View(model);            

            try
            {
                string teamId = await teamService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = teamId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model = await teamService.AddAsync();
                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid team ID.");
            }

            try
            {
                var team = await teamService.EditByIdAsync(id);
                return View(team);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeamEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isUpdated = await teamService.EditByModelAsync(model);
            if (!isUpdated)
            {
                ModelState.AddModelError("", "Update failed. Please try again.");
                return View(model);
            }

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid team ID.");
            }

            bool isDeleted = await teamService.SoftDeleteAsync(id);
            if (!isDeleted)
            {
                return BadRequest("Deletion failed. Please try again.");
            }

            return RedirectToAction(nameof(All));
        }
    }
}
