namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Application.Services.Interfaces;
    using Models.JobDone;
    using static Common.EntityValidationConstants.CalculationAction;
    using ElProApp.Common;

    [Authorize]
    public class JobDoneController(
        IJobDoneService jobDoneService,
        IEarningsCalculationService earningsCalculationService
    ) : Controller
    {
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> All() => View(await jobDoneService.GetAllAsync());
        

        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Add()
            => View(await jobDoneService.AddAsync());

        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Add(JobDoneInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var freshModel = await jobDoneService.AddAsync();

                freshModel.Name = model.Name;
                freshModel.StartDate = model.StartDate;
                freshModel.EndDate = model.EndDate;
                freshModel.DaysForJob = model.DaysForJob;
                freshModel.TeamId = model.TeamId;
                freshModel.BuildingId = model.BuildingId;
                freshModel.Jobs = model.Jobs;

                return View(freshModel);
            }

            string jobDoneId = await jobDoneService.AddAsync(model);

            return RedirectToAction(nameof(Details), new { id = jobDoneId });
        }

        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
            => View(await jobDoneService.GetByIdAsync(id));

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
            => View(await jobDoneService.EditByIdAsync(id));

        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpPost]
        public async Task<IActionResult> Edit(JobDoneEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldJobDone = await jobDoneService.GetByIdAsync(model.Id.ToString());

            await earningsCalculationService.CalculateMoneyAsync(
                oldJobDone.TeamId,
                oldJobDone.Jobs,
                oldJobDone.Id,
                oldJobDone.DaysForJob,
                Remove
            );

            bool edited = await jobDoneService.EditByModelAsync(model);

            if (!edited)
            {
                return View(model);
            }

            await earningsCalculationService.CalculateMoneyAsync(
                model.TeamId,
                model.Jobs,
                model.Id,
                model.DaysForJob,
                EntityValidationConstants.CalculationAction.Add
            );

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var jobDone = await jobDoneService.GetByIdAsync(id);

            await earningsCalculationService.CalculateMoneyAsync(
                jobDone.TeamId,
                jobDone.Jobs,
                jobDone.Id,
                jobDone.DaysForJob,
                Remove);

            bool isDeleted = await jobDoneService.SoftDeleteAsync(id, jobDone.TeamId.ToString());

            if (!isDeleted)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(All));
        }
    }
}
