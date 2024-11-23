namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IEmployeeTeamMappingService
    {
        /// <summary>
        /// Retrieves all mappings between a specific employee and teams by the employee's ID.
        /// </summary>
        /// <param name="id">The unique ID of the employee.</param>
        /// <returns>
        /// A collection of <see cref="EmployeeTeamMapping"/> objects associated with the specified employee.
        /// </returns>
        ICollection<EmployeeTeamMapping> GetAllByEmployeeId(string id);

        /// <summary>
        /// Retrieves all employee-team mappings as an asynchronous collection.
        /// </summary>
        /// <returns>
        /// A collection of all <see cref="EmployeeTeamMapping"/> objects in the system.
        /// </returns>
        Task<ICollection<EmployeeTeamMapping>> GetAllAsync();

        /// <summary>
        /// Retrieves all employee-team mappings as a queryable collection for advanced filtering and querying.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="EmployeeTeamMapping"/> objects.
        /// </returns>
        IQueryable<EmployeeTeamMapping> GetAllAttached();

        /// <summary>
        /// Adds a new mapping between an employee and a team.
        /// </summary>
        /// <param name="employeeId">The unique ID of the employee.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// The created <see cref="EmployeeTeamMapping"/> object.
        /// </returns>
        Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid teamId);

        /// <summary>
        /// Retrieves all mappings for a specific team by the team's ID.
        /// </summary>
        /// <param name="id">The unique ID of the team.</param>
        /// <returns>
        /// A collection of <see cref="EmployeeTeamMapping"/> objects associated with the specified team.
        /// </returns>
        Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid id);

        /// <summary>
        /// Checks if a mapping exists between a specific employee and a team.
        /// </summary>
        /// <param name="employeeId">The unique ID of the employee.</param>
        /// <param name="teamId">The unique ID of the team.</param>
        /// <returns>
        /// True if the mapping exists, otherwise false.
        /// </returns>
        bool Any(Guid employeeId, Guid teamId);

        /// <summary>
        /// Removes a specific mapping between an employee and a team.
        /// </summary>
        /// <param name="mapping">The <see cref="EmployeeTeamMapping"/> object representing the mapping to remove.</param>
        /// <returns>
        /// A boolean indicating whether the removal was successful.
        /// </returns>
        Task<bool> RemoveAsync(EmployeeTeamMapping mapping);
    }
}
