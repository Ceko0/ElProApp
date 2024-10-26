namespace ElProApp.Web.Controllers
{   
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    
    using Data;
    using Models.Employee;
    using Data.Models;
    using System.Security.Claims;

    [Authorize]
    public class EmployeeController(ElProAppDbContext data , UserManager<IdentityUser> userManager) : Controller
    {
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

            if(data.Employees.Any(e => e.UserId == userId))
            {
                ModelState.AddModelError(string.Empty, "Вече имате създаден служител");
                return View(model);
            }

            var currentUser = await userManager.FindByIdAsync(userId);

            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Wages = model.Wages,
                UserId = userId ?? string.Empty,
                User = currentUser = null!
            };


            data.Employees.Add(employee);
            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
