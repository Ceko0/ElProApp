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
            try
            {
                string employeeId = await employeeService.AddAsync(model);
                return RedirectToAction(nameof(Details),new { id = employeeId });
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
            var model = await employeeService.GetEmployeeByIdAsync(id);

            if (model == null)
            {
                return BadRequest("Invalid Employee.");
            }

            return View(model);
        }
    }
}
