using ElProApp.Web.Models.JobDone;

namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
    using ElProApp.Services.Data;

    [Authorize]
    public class JobDoneController(IJobDoneService _jobDoneService) : Controller
    {
        private readonly IJobDoneService jobDoneService = _jobDoneService;

        [HttpGet]
        public async Task<IActionResult> All() 
            => View(await jobDoneService.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> Add() => View(await jobDoneService.AddAsync());

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

        [HttpGet]
        public async Task<IActionResult> Details(string id) 
            => View( await jobDoneService.GetByIdAsync(id));

        [HttpGet]
        public IActionResult Edit(string id)
            => View(new JobDoneEditInputModel());

        [HttpPost]
        public async Task<IActionResult> Edit(JobDoneEditInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (await jobDoneService.EditByModelAsync(model))
                    return RedirectToAction(nameof(Details), new { id = model.Id});
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
            bool isDeleted = await jobDoneService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }
            ModelState.AddModelError("", "Failed to delete Job Done.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
