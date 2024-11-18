namespace ElProApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    using Models.Mappings;
    using static Common.EntityValidationErrorMessage.Employee;
    using static Common.EntityValidationErrorMessage.Master;
    using static Common.EntityValidationConstants.Employee;

    /// <summary>
    /// Represents an Employee entity with personal information, salary details, and associations to user accounts and teams.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Unique identifier for the employee.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Comment("Unique identifier for the employee.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// First name of the employee, constrained by a maximum length.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The first name of the employee with a maximum of 20 characters.")]
        [PersonalData]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Last name of the employee, constrained by a maximum length.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The last name of the employee with a maximum of 20 characters.")]
        [PersonalData]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// The employee's wage, specified with a range.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassageWages)]
        [Display(Name = "Заплата")]
        [Comment("The wages of the employee with up to 6 digits before the decimal point and up to 2 digits after.")]
        public decimal Wages { get; set; } = 0.0m;

        /// <summary>
        /// The total money the employee is set to receive, must be positive.
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [Display(Name = "Сума, която служителят трябва да получи")]
        [Comment("The money the employee has to take, must be a positive value.")]
        public decimal MoneyToTake { get; set; } = 0.0m;

        /// <summary>
        /// Indicates if the employee is active (false) or soft deleted (true).
        /// <para>This property helps in managing logical deletion without removing records from the database.</para>
        /// </summary>
        [Comment("Indicates if the employee is active or soft deleted.")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The ID of the user account associated with this employee.
        /// </summary>
        [Comment("Foreign key representing the user account associated with this employee.")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the IdentityUser associated with this employee.
        /// </summary>
        [Comment("Navigation property to the IdentityUser associated with this employee.")]
        public IdentityUser User { get; set; } = null!;
    }
}
