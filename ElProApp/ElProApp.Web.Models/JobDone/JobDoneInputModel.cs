namespace ElProApp.Web.Models.JobDone
{
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;
    using Services.Mapping;
    using static Common.EntityValidationErrorMessage.JobDobe;
    using static Common.EntityValidationErrorMessage.Master;
    using static ElProApp.Common.EntityValidationConstants.JobDone;

    public class JobDoneInputModel : IMapTo<JobDone> , IMapFrom<JobDone>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Display(Name = "Име")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Началната дата е задължителна.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Крайната дата е задължителна.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        public int DaysForJob { get; set; }

        public Guid TeamId { get; set; }

        public Guid BuildingId { get; set; }

        public Dictionary<Guid, decimal> Jobs { get; set; } = new();

        public virtual ICollection<Job> JobsList { get; set; } = new List<Job>();
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
    }
}
