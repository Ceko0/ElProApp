namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;

    using ElProApp.Data;
    using Models.Employee;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;

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
        /// <returns>View with the list of employees.</returns>
        [HttpGet]
        public async Task<IActionResult> All()
            => View(await employeeService.GetAllAsync());

        /// <summary>
        /// Displays the form for adding a new employee.
        /// </summary>
        /// <returns>View for adding an employee.</returns>
        [HttpGet]
        public IActionResult Add()
            => View(new EmployeeInputModel());

        /// <summary>
        /// Processes the request to add a new employee.
        /// </summary>
        /// <param name="model">The employee model.</param>
        /// <returns>Redirects to employee details or stays on the page if there's an error.</returns>
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeInputModel model)
        {
            try
            {
                string employeeId = await employeeService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = employeeId });
            }
            catch (InvalidOperationException)
            {
                return View(model);
            }
        }

        /// <summary>
        /// Displays details for an employee by ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>View with employee details.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
            => View(await employeeService.GetByIdAsync(id));

        /// <summary>
        /// Displays the form for editing an employee.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>View for editing the employee.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
            => View(await employeeService.EditByIdAsync(id));

        /// <summary>
        /// Processes the request to edit an employee.
        /// </summary>
        /// <param name="model">The model with new employee data.</param>
        /// <returns>Redirects to employee details or stays on the page if there's an error.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeEditInputModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (await employeeService.EditByModelAsync(model)) 
                    return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch
            {
                RedirectToAction(nameof(All));
            }
            return View(model);
        }

        /// <summary>
        /// Processes the request to soft delete an employee.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>Redirects to the employee list.</returns>
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            bool isDeleted = await employeeService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }
            ModelState.AddModelError("", "Failed to delete employee.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
