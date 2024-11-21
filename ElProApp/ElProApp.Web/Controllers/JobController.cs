namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using Models.Job;


    public class JobController(IJobService _jobService) : Controller
    {
        private readonly IJobService jobService = _jobService;

        [HttpGet]
        public async Task<IActionResult> All()
            => View(await jobService.GetAllAsync());

        [HttpGet]
        public IActionResult Add(string id)
            => View(new JobInputModel());

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

        [HttpGet]
        public async Task<IActionResult> Details(string id) => View(await jobService.GetByIdAsync(id));

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await jobService.EditByIdAsync(id);
            
            return View(model);
        }

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

        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            bool isDeleted = await jobService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }
            ModelState.AddModelError("", "Failed to delete Job Done.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
