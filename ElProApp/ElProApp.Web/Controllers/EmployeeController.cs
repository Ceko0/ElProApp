namespace ElProApp.Web.Controllers
{   
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data;
    using Models.Employee;
    using Data.Models;
    using System.Security.Claims;
    using ElProApp.Services.Data.Interfaces;

    [Authorize]
    public class EmployeeController(ElProAppDbContext data,
                              UserManager<IdentityUser> userManager,
                              IEmployeeService employeeService) : Controller
    {
        private readonly ElProAppDbContext data = data;
        private readonly UserManager<IdentityUser> userManager = userManager;
        private readonly IEmployeeService employeeService = employeeService;

        [HttpGet]
        public IActionResult Add() 
        {
            return View(new EmployeeInputModel());
        }        
        
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeInputModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Неуспешно извличане на UserId. Опитай отново.");
                return View(model);
            }

            try
            {
                await employeeService.AddAsync(model, userId);
                return RedirectToAction(nameof(Details));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!Guid.TryParse(id, out Guid validId))
            {
                return BadRequest("Invalid ID format.");
            }

            var model = await employeeService.GetEmployeeByIdAsync(validId);

            if (model == null)
            {
                return BadRequest("Invalid Employee.");
            }

            return View(model);
        }
        private string? GetUserId() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
