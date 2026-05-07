namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Application.Services.Interfaces;
    using Models.JobDone;

    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    /// <summary>
    /// Controller responsible for managing job-done operations.
    /// </summary>
    [Authorize]
    public class JobDoneController(IJobDoneService jobDoneService)
        : Controller
    {
        /// <summary>
        /// Retrieves all job-done records.
        /// </summary>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> All()
            => View(await jobDoneService.GetAllAsync());

        /// <summary>
        /// Displays the create job-done form.
        /// </summary>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Add()
            => View(await jobDoneService.AddAsync());

        /// <summary>
        /// Creates a new job-done record.
        /// </summary>
        /// <param name="model">The input model.</param>

        [HttpPost]
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        public async Task<IActionResult> Add(JobDoneInputModel model)
        {
            model.Name = $"От {model.StartDate:dd.MM.yyyy} до {model.EndDate:dd.MM.yyyy} Екип : {model.TeamName}";

            if (!ModelState.IsValid)
            {
                var freshModel = await jobDoneService.AddAsync();

                freshModel.StartDate = model.StartDate;
                freshModel.EndDate = model.EndDate;
                freshModel.DaysForJob = model.DaysForJob;
                freshModel.TeamId = model.TeamId;
                freshModel.BuildingId = model.BuildingId;
                freshModel.Materials = model.Materials;

                return View(freshModel);
            }

            string jobDoneId = await jobDoneService.AddAsync(model);

            return RedirectToAction(nameof(Details), new { id = jobDoneId });
        }

        /// <summary>
        /// Displays details for a job-done record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
            => View(await jobDoneService.GetByIdAsync(id));

        /// <summary>
        /// Displays the edit form.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
            => View(await jobDoneService.EditByIdAsync(id));

        /// <summary>
        /// Updates a job-done record.
        /// </summary>
        /// <param name="model">The edit model.</param>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpPost]
        public async Task<IActionResult> Edit(JobDoneEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var freshModel = await jobDoneService.EditByIdAsync(model.Id.ToString());

                freshModel.Name = model.Name;
                freshModel.DaysForJob = model.DaysForJob;
                freshModel.Materials = model.Materials ?? new List<MaterialInputPair>();

                return View(freshModel);
            }

            await jobDoneService.EditByModelAsync(model);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        /// <summary>
        /// Soft deletes a job-done record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var jobDone = await jobDoneService.GetByIdAsync(id);

            bool isDeleted = await jobDoneService
                .SoftDeleteAsync(id, jobDone.TeamId.ToString());

            if (!isDeleted)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(All));
        }
    }
}