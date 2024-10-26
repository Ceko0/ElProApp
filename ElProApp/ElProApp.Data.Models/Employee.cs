namespace ElProApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidationErrorMessage.Employee;
    using static Common.EntityValidationErrorMessage.Master;
    using static Common.EntityValidationConstants.Employee;
    using Microsoft.EntityFrameworkCore;
    using ElProApp.Data.Models.Mappings;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;

    public class Employee
    {
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Comment("Unique identifier for the employee.")]
        public Guid Id { get; set; } = new();

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The first name of the employee with a maximum of 20 characters.")]
        [PersonalData]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The last name of the employee with a maximum of 20 characters.")]
        [PersonalData]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassageWages)]
        [Display(Name = "Заплата")]
        [Comment("The wages of the employee with up to 4 digits before the decimal point and up to 2 digits after.")]
        public decimal Wages { get; set; } = 0.0m;

        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [Display(Name = "Сума, която служителят трябва да получи")]
        [Comment("The money the employee has to take, must be a positive value.")]
        public decimal MoneyToTake { get; set; } = 0.0m;

        [Comment("Foreign key representing the user account associated with this employee.")]
        public string UserId { get; set; } = null!;

        [Comment("Navigation property to the IdentityUser associated with this employee.")]
        public IdentityUser User { get; set; } = null!;

        [Comment("Collection representing the many-to-many relationship between employees and teams.")]
        public virtual ICollection<EmployeeTeamMapping> TeamsEmployeeBelongsTo { get; set; } = [];
    }
}
