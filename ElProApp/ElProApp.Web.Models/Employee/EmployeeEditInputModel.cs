namespace ElProApp.Web.Models.Employee
{
    using System.ComponentModel.DataAnnotations;
    using Services.Mapping;

    using static Common.EntityValidationConstants.Employee;
    using static Common.EntityValidationErrorMessage.Employee;
    using static Common.EntityValidationErrorMessage.Master;
    using Data.Models;

    /// <summary>
    /// ViewModel for editing employee details.
    /// Implements mapping to the Employee entity.
    /// </summary>
    public class EmployeeEditInputModel : IMapTo<Employee>, IMapFrom<Employee>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the employee.
        /// This field is required and must not exceed a maximum length defined in constants.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name of the employee.
        /// This field is required and must not exceed a maximum length defined in constants.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the wages of the employee.
        /// This field is required and must be within the specified range.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassageWages)]
        [Display(Name = "Заплата")]
        public decimal Wages { get; set; }
    }
}
