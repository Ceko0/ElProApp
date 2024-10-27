namespace ElProApp.Web.Models.Employee
{
    using Services.Mapping;

    using Data.Models.Mappings;

    /// <summary>
    /// ViewModel representing an employee in the application.
    /// Implements mapping from the Data.Models.Employee entity.
    /// </summary>
    public class EmployeeAllViewModel : IMapFrom<Data.Models.Employee>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the first name of the employee.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name of the employee.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the wages of the employee.
        /// </summary>
        public decimal Wages { get; set; }

        /// <summary>
        /// Gets or sets the amount of money the employee is entitled to take.
        /// </summary>
        public decimal MoneyToTake { get; set; }

        /// <summary>
        /// Gets or sets the username associated with the employee.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of teams that the employee belongs to.
        /// </summary>
        public ICollection<EmployeeTeamMapping> TeamsEmployeeBelongsTo { get; set; } = new List<EmployeeTeamMapping>();
    }
}
