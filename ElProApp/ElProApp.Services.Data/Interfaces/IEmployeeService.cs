namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Employee;

    public interface IEmployeeService
    {
        /// <summary>
        /// Adds a new employee based on the provided input model.
        /// </summary>
        /// <param name="model">The model containing employee data.</param>
        /// <returns>The ID of the newly added employee.</returns>
        Task<string> AddAsync(EmployeeInputModel model);

        /// <summary>
        /// Retrieves an employee by their ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>The employee view model, or null if not found.</returns>
        Task<EmployeeViewModel?> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves the edit model for an employee by their ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>The edit input model for the employee.</returns>
        Task<EmployeeEditInputModel> EditByIdAsync(string id);

        /// <summary>
        /// Edits an existing employee using the provided edit input model.
        /// </summary>
        /// <param name="model">The model containing updated employee data.</param>
        /// <returns>A boolean indicating success or failure of the edit operation.</returns>
        Task<bool> EditByModelAsync(EmployeeEditInputModel model);

        /// <summary>
        /// Retrieves all employees as a list.
        /// </summary>
        /// <returns>A collection of employee view models.</returns>
        Task<ICollection<EmployeeViewModel>> GetAllAsync();

        IQueryable<Employee> GetAllAttached();

        /// <summary>
        /// Soft deletes an employee by their ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>A boolean indicating success or failure of the delete operation.</returns>
        Task<bool> SoftDeleteAsync(string id);
        Employee GetByUserId(string id);
    }
}
