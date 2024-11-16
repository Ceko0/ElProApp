namespace ElProApp.Web.Models.Employee
{
    using Services.Mapping;
    using Data.Models.Mappings;
    using Data.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using ElProApp.Web.Models.Team;

    /// <summary>
    /// ViewModel for displaying employee details.
    /// Implements mapping from the Employee entity.
    /// </summary>
    public class EmployeeViewModel : IMapFrom<Employee>, IHaveCustomMappings
    {
        /// <summary>
        /// Gets or sets the unique identifier of the employee.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets the unique identifier of the user associated with the employee.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the username of the employee.
        /// </summary>
        public IdentityUser User { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of teams that the employee belongs to.
        /// </summary>
        public ICollection<EmployeeTeamMapping> TeamsEmployeeBelongsTo { get; set; } = new List<EmployeeTeamMapping>();

        /// <summary>
        /// Creates custom mappings for the EmployeeViewModel.
        /// Maps the UserName property from the associated User entity.
        /// </summary>
        /// <param name="configuration">The configuration profile for AutoMapper.</param>
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Employee, EmployeeViewModel>()
                .ForMember(d => d.User, x => x.MapFrom(s => s.User));
            configuration.CreateMap<Employee, EmployeeViewModel>()
                .ForMember(d => d.TeamsEmployeeBelongsTo, x => x.MapFrom(s => s.TeamsEmployeeBelongsTo));
        }
    }
}
