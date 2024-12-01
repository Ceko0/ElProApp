namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using Models.Job;

    /// <summary>
    /// Controller for managing job entries.
    /// </summary>
    [Authorize]
    public class JobController(IJobService _jobService) : Controller
    {
        private readonly IJobService jobService = _jobService;

        /// <summary>
        /// Displays a list of all jobs.
        /// </summary>
        /// <returns>A view with the list of jobs.</returns>
        [HttpGet]
        public async Task<IActionResult> All()
            => View(await jobService.GetAllAsync());

        /// <summary>
        /// Displays the form for adding a new job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID used for job-related associations, if any.</param>
        /// <returns>A view for adding a job.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public IActionResult Add(string id)
            => View(new JobInputModel());

        /// <summary>
        /// Processes the request to add a new job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The job input model containing job details.</param>
        /// <returns>Redirects to job details or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpPost]
        public async Task<IActionResult> Add(JobInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                string jobDoneId = await jobService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = jobDoneId });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        /// <summary>
        /// Displays the details of a job by its ID.
        /// </summary>
        /// <param name="id">The ID of the job.</param>
        /// <returns>A view with the job details.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
            => View(await jobService.GetByIdAsync(id));

        /// <summary>
        /// Displays the form for editing a job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the job to edit.</param>
        /// <returns>A view for editing the job.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await jobService.EditByIdAsync(id);
            return View(model);
        }

        /// <summary>
        /// Processes the request to edit an existing job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The edited job model with updated job details.</param>
        /// <returns>Redirects to job details or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> Edit(JobEditInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (await jobService.EditByModelAsync(model))
                    return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        /// <summary>
        /// Processes the request to soft delete a job.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the job to delete.</param>
        /// <returns>Redirects to the list of jobs or displays an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            bool isDeleted = await jobService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }
            ModelState.AddModelError("", "Failed to delete the job.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
