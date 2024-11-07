namespace ElProApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationErrorMessage.JobDobe;
    using static Common.EntityValidationErrorMessage.Master;
    using Mappings;

    /// <summary>
    /// Represents a completed job record, including details about the job, quantity completed, and time spent.
    /// </summary>
    public class JobDone
    {
        /// <summary>
        /// Unique identifier for the job done record.
        /// </summary>
        [Required]
        [Comment("Unique identifier for the job done record.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// Foreign key representing the job that has been completed.
        /// </summary>
        [Required]
        [Comment("Foreign key for the job being done.")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Navigation property to the job associated with this completion record.
        /// </summary>
        [Required]
        [Comment("The job associated with this record. Represents the job that has been completed.")]
        public Job Job { get; set; } = null!;

        /// <summary>
        /// Quantity of work completed, with constraints on format and required positive value.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, double.MaxValue, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        [Comment("Quantity of work completed.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Number of days spent completing the job, restricted to between 1 and 30 days.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(1, 30, ErrorMessage = ErrorMassageDaysForJob)]
        [Comment("Number of days spent completing the job.")]
        public int DaysForJob { get; set; }

        /// <summary>
        /// Foreign key for the team that completed the job. 
        /// If you want to use many-to-many mapping, remove this property.
        /// </summary>
        [Required]
        [Comment("Foreign key for the team responsible for completing the job.")]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Collection of mappings between teams and this job, indicating which teams participated in completing the job.
        /// </summary>
        [Comment("Collection representing teams that completed the job, mapped in a many-to-many relationship.")]
        public virtual ICollection<JobDoneTeamMapping> TeamsDoTheJob { get; set; } = new List<JobDoneTeamMapping>();
    }
}
