namespace ElProApp.Web.Models.JobDone
{
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using static ElProApp.Common.EntityValidationErrorMessage.JobDobe;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;

    public class JobDoneInputModel : IMapTo<JobDone>
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        public decimal quantity { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        //[Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        public Guid JobId { get; set; }

        public Job Job { get; set; } = new();

        public virtual ICollection<JobDoneTeamMapping> TeamsDoTheJob { get; set; } = new List<JobDoneTeamMapping>();
    }
}
