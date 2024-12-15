namespace ElProApp.Web.Models.JobDone
{
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;
    using static ElProApp.Common.EntityValidationErrorMessage.JobDobe;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;
    using static ElProApp.Common.EntityValidationConstants.JobDone;

    public class JobDoneInputModel : IMapTo<JobDone> , IMapFrom<JobDone>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(nameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        public Guid JobId { get; set; }

        public Guid TeamId { get; set; }

        public Guid BuildingId { get; set; }

        public virtual ICollection<Job> jobs { get; set; } = new List<Job>();
        public virtual ICollection<Team> teams { get; set; } = new List<Team>();
        public virtual ICollection<Building> buildings { get; set; } = new List<Building>();
    }
}
