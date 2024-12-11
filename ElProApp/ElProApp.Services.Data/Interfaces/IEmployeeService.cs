namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using Web.Models.Employee;

    public interface IEmployeeService
    {
        /// <summary>
        /// Adds a new employee based on the provided input model.
        /// </summary>
        /// <param name="model">The model containing the details of the employee to add.</param>
        /// <returns>
        /// The unique ID of the newly added employee as a string.
        /// </returns>
        Task<string> AddAsync(EmployeeInputModel model);

        /// <summary>
        /// Retrieves an employee by their unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the employee.</param>
        /// <returns>
        /// The <see cref="EmployeeViewModel"/> representing the employee's data, or null if the employee does not exist.
        /// </returns>
        Task<EmployeeViewModel?> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves an editable model for the employee by their unique ID.
        /// This model is used for populating edit forms in the application.
        /// </summary>
        /// <param name="id">The unique ID of the employee.</param>
        /// <returns>
        /// An <see cref="EmployeeEditInputModel"/> containing the employee's editable data.
        /// </returns>
        Task<EmployeeEditInputModel> EditByIdAsync(string id);

        /// <summary>
        /// Updates an existing employee's details using the provided edit input model.
        /// </summary>
        /// <param name="model">The input model containing updated employee details.</param>
        /// <returns>
        /// True if the update was successful, false otherwise.
        /// </returns>
        Task<bool> EditByModelAsync(EmployeeEditInputModel model);

        /// <summary>
        /// Retrieves all employees as an asynchronous collection of view models.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="EmployeeViewModel"/> objects representing all employees in the system.
        /// </returns>
        Task<ICollection<EmployeeViewModel>> GetAllAsync();

        /// <summary>
        /// Retrieves all employees as a queryable collection.
        /// This method is useful for filtering, sorting, and querying data at the database level.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="Employee"/> objects.
        /// </returns>
        IQueryable<Employee> GetAllAttached();

        /// <summary>
        /// Soft deletes an employee by marking them as inactive in the system.
        /// This does not permanently remove the employee from the database.
        /// </summary>
        /// <param name="id">The unique ID of the employee to soft delete.</param>
        /// <returns>
        /// True if the soft delete was successful, false otherwise.
        /// </returns>
        Task<bool> SoftDeleteAsync(string id);

        /// <summary>
        /// Retrieves an employee entity by their associated user ID.
        /// This is used to link Identity users with employee records.
        /// </summary>
        /// <param name="id">The unique ID of the Identity user.</param>
        /// <returns>
        /// The <see cref="Employee"/> entity associated with the given user ID.
        /// </returns>
        Employee GetByUserId(string id);

        /// <summary>
        /// Adds a new employee record specifically for an administrative user.
        /// This method is used during admin setup or user-role assignments.
        /// </summary>
        /// <param name="firstName">The first name of the admin employee.</param>
        /// <param name="lastName">The last name of the admin employee.</param>
        /// <param name="identityUserId">The unique ID of the associated Identity user.</param>
        /// <returns>
        /// The unique ID of the newly added admin employee as a string.
        /// </returns>
        Task<string> AddAdminEmployeeAsync(string firstName, string lastName, string identityUserId);

        Task<bool> SaveChangesAsync();

    }
}
