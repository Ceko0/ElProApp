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
                return RedirectToAction(nameof(Index));
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
            Guid validId = Guid.Empty;
            bool isValid =IdValidation(id,ref validId);

            if(!isValid) return BadRequest("Invalid ID format.");

            var entity = await data.Employees.FirstOrDefaultAsync(e => e.Id == validId);

            if (entity == null) return BadRequest("Invalid Employee.");

            var model = new EmployeeViewModel()
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Wages = entity.Wages,
                MoneyToTake = entity.MoneyToTake,
                UserId = entity.UserId,
                UserName = entity.User.UserName ?? string.Empty,
                TeamsEmployeeBelongsTo = entity.TeamsEmployeeBelongsTo
            };

            return View();
        }

        private static bool IdValidation(string id, ref Guid validId)
        {
            bool isValidId = Guid.TryParse(id, out  validId);
            
            return isValidId;
        }

        private string? GetUserId() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
