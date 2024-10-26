namespace ElProApp.Web.Models.Employee
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Employee;
    using static Common.EntityValidationErrorMessage.Employee;
    using static Common.EntityValidationErrorMessage.Master;
    using Data.Models;
    using Services.Mapping;

    public class EmployeeInputModel : IMapFrom<Employee>
    {
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassageWages)]
        [Display(Name = "Заплата")]
        public decimal Wages { get; set; }
    }
}
