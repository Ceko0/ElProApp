namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.JobDone;
    using static Common.ApplicationConstants;

    /// <summary>
    /// Controller for managing completed jobs.
    /// </summary>
    [Authorize]
    public class JobDoneController(IJobDoneService jobDoneService, IServiceProvider serviceProvider) : Controller
    {
     

        /// <summary>
        /// Displays a list of all completed jobs.
        /// </summary>
        /// <returns>A view with the list of completed jobs.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (User.IsInRole(AdminRoleName) || User.IsInRole(OfficeManagerRoleName) || User.IsInRole(TechnicianRoleName))
                return RedirectToAction("AllJobDones", "Admin", new { area = "admin" });

            return View(await jobDoneService.GetAllAsync());
        }
        /// <summary>
        /// Displays the form for adding a new completed job.
        /// Accessible only by administrators.
        /// </summary>
        /// <returns>A view for adding a completed job.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Add() => View(await jobDoneService.AddAsync());

        /// <summary>
        /// Processes the request to add a new completed job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The completed job input model.</param>
        /// <returns>Redirects to job details or displays the same view if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpPost]
        public async Task<IActionResult> Add(JobDoneInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                string jobDoneId = await jobDoneService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = jobDoneId });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        /// <summary>
        /// Displays details of a completed job by its ID.
        /// </summary>
        /// <param name="id">The ID of the completed job.</param>
        /// <returns>A view with the completed job details.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
            => View(await jobDoneService.GetByIdAsync(id));

        /// <summary>
        /// Displays the form for editing a completed job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the completed job to edit.</param>
        /// <returns>A view for editing the completed job.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await jobDoneService.EditByIdAsync(id);

            return View(model);
        }

        /// <summary>
        /// Processes the request to edit a completed job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The edited completed job input model.</param>
        /// <returns>Redirects to job details or displays the same view if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpPost]
        public async Task<IActionResult> Edit(JobDoneEditInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (await jobDoneService.EditByModelAsync(model))
                    return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        /// <summary>
        /// Processes the request to soft delete a completed job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the completed job to delete.</param>
        /// <returns>Redirects to the list of completed jobs.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id, string teamId)
        {
            bool isDeleted = await jobDoneService.SoftDeleteAsync(id , teamId);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }
            ModelState.AddModelError("", "Failed to delete the completed job.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
