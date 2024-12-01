namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using ElProApp.Data;
    using Models.Employee;
    using ElProApp.Services.Data.Interfaces;

    /// <summary>
    /// Controller for managing employee entries.
    /// </summary>
    [Authorize]
    public class EmployeeController(ElProAppDbContext _data,
                                  UserManager<IdentityUser> _userManager,
                                  IEmployeeService _employeeService) : Controller
    {
        private readonly ElProAppDbContext data = _data;
        private readonly UserManager<IdentityUser> userManager = _userManager;
        private readonly IEmployeeService employeeService = _employeeService;

        /// <summary>
        /// Displays a list of all employees.
        /// </summary>
        /// <returns>A view with the list of employees.</returns>
        [HttpGet]
        public async Task<IActionResult> All()
            => View(await employeeService.GetAllAsync());

        /// <summary>
        /// Displays the form for adding a new employee.
        /// Accessible only by administrators.
        /// </summary>
        /// <returns>A view for adding an employee.</returns>
        [HttpGet]
        public IActionResult Add()
            => View(new EmployeeInputModel());

        /// <summary>
        /// Processes the request to add a new employee.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The employee model containing details to be added.</param>
        /// <returns>Redirects to employee details or stays on the page if there's an error.</returns>
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeInputModel model)
        {
            try
            { 
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string employeeId = await employeeService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = employeeId });
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "An employee already exists for this user.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        /// <summary>
        /// Displays details for an employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>A view with employee details.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!CheckStringId(id)) return BadRequest("Employee ID cannot be empty.");

            var employee = await employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound("Employee not found.");

            return View(employee);
        }

        /// <summary>
        /// Displays the form for editing an employee.
        /// </summary>
        /// <param name="id">The ID of the employee to edit.</param>
        /// <returns>A view for editing the employee.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!CheckStringId(id)) return BadRequest("Employee ID cannot be empty.");

            var employee = await employeeService.EditByIdAsync(id);
            if (employee == null) return NotFound("Employee not found.");

            return View(employee);
        }

        /// <summary>
        /// Processes the request to edit an employee.
        /// </summary>
        /// <param name="model">The model with updated employee data.</param>
        /// <returns>Redirects to employee details or stays on the page if there's an error.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (await employeeService.EditByModelAsync(model))
                {
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update employee.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Processes the request to soft delete an employee.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>Redirects to the employee list or displays an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            if (!CheckStringId(id)) throw new BadHttpRequestException("Employee ID cannot be empty.");

            var employee = await employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            bool isDeleted = await employeeService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Checks if the provided employee ID is valid (not null or empty).
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>True if the ID is valid, otherwise false.</returns>
        private bool CheckStringId(string id)
        {
            return !string.IsNullOrWhiteSpace(id);
        }
    }
}
