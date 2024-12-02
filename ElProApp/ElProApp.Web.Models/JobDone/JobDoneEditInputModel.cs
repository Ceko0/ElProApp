namespace ElProApp.Web.Models.JobDone
{ 
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;    
    using Services.Mapping;
    using static Common.EntityValidationErrorMessage.JobDobe;
    using static Common.EntityValidationErrorMessage.Master;
    using static Common.EntityValidationConstants.JobDone;

    public class JobDoneEditInputModel : IMapTo<JobDone>, IMapFrom<JobDone>
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(nameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]      
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        public decimal quantity { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        public Guid JobId { get; set; }

        public Job Job { get; set; } = new();

        public Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;

        public Guid TeamId { get; set; }

        public Team Team { get; set; } = null!;
    }
}
