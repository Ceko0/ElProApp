namespace ElProApp.Web.Models.Employee
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Employee;
    using static Common.EntityValidationErrorMessage.Employee;
    using static Common.EntityValidationErrorMessage.Master;

    public class EmployeeInputModel
    {
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassageWages)]
        [Display(Name = "Заплата")]
        public decimal Wages { get; set; }
    }
}
