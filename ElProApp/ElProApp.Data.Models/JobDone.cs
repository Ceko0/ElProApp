namespace ElProApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationErrorMessage.JobDobe;
    using static Common.EntityValidationErrorMessage.Master;
    using static Common.EntityValidationConstants.JobDone;
    using ElProApp.Services.Data.Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a completed job record, including details about the job, quantity completed, and time spent.
    /// </summary>
    public class JobDone : IDeletableEntity
    {
        /// <summary>
        /// Unique identifier for the job done record.
        /// </summary>
        [Required]
        [Comment("Unique identifier for the job done record.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// The name of the jobDone, constrained by a maximum length.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(nameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The name of the job with a maximum of 50 characters.")]
        public string Name { get; set; } = null!;

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
        /// Foreign key for the building where was completing the job. 
        /// If you want to use many-to-many mapping, remove this property.
        /// </summary>
        [Required]
        [Comment("Foreign key for the building where was completing the job.")]
        public Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;

        /// <summary>
        /// Indicates if the jobdone is active (false) or soft deleted (true).
        /// <para>This property helps in managing logical deletion without removing records from the database.</para>
        /// </summary>
        [Comment("Indicates if the jobdone is active or soft deleted.")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;

        /// <summary>
        /// The date when the record was deleted (logically deleted).
        /// </summary>
        [Comment("The date when the record was deleted (logically deleted).")]
        [Column(TypeName = "date")]
        public DateTime? DeletedDate { get; set; }
    }
}
