namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.Team;
    using static Common.ApplicationConstants;

    /// <summary>
    /// Controller for managing team entries and operations.
    /// </summary>
    [Authorize]
    public class TeamController(ITeamService _teamService) : Controller
    {
        private readonly ITeamService teamService = _teamService;

        /// <summary>
        /// Displays a list of all teams.
        /// </summary>
        /// <returns>A view with the list of all teams.</returns>
        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (User.IsInRole(AdminRoleName) || User.IsInRole(OfficeManagerRoleName))
                return RedirectToAction("AllTeams", "Admin", new { area = "admin" });

            var teams = await teamService.GetAllAsync();
            return View(teams);
        }
        /// <summary>
        /// Retrieves the details of a specific team by its ID.
        /// </summary>
        /// <param name="id">The ID of the team to retrieve details for.</param>
        /// <returns>Returns a view displaying the team's details if found, otherwise returns a BadRequest or NotFound result.</returns>

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

        /// <summary>
        /// Displays the form for adding a new team.
        /// Accessible only by administrators.
        /// </summary>
        /// <returns>A view for adding a new team.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {

            var model = await teamService.AddAsync();

            return View(model);
        }

        /// <summary>
        /// Processes the request to add a new team.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The team model containing the new team details.</param>
        /// <returns>Redirects to the team details view or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
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

        /// <summary>
        /// Displays the form for editing an existing team.
        /// </summary>
        /// <param name="id">The ID of the team to edit.</param>
        /// <returns>A view for editing the team details.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
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

        /// <summary>
        /// Processes the request to update the team details.
        /// </summary>
        /// <param name="model">The model with updated team data.</param>
        /// <returns>Redirects to the team list or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
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

        /// <summary>
        /// Processes the request to soft delete a team.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the team to delete.</param>
        /// <returns>Redirects to the team list or shows an error message.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
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
