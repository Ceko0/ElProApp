namespace ElProApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Job;
    using static Common.EntityValidationErrorMessage.Job;
    using static Common.EntityValidationErrorMessage.Master;
    using ElProApp.Services.Data.Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a Job entity with details about job name, price, and associated work done by teams.
    /// </summary>
    public class Job : IDeletableEntity
    {
        /// <summary>
        /// Unique identifier for the job.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Comment("Unique identifier for the job.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// The name of the job, constrained by a maximum length.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(nameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The name of the job with a maximum of 50 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Price of the job, following a set range and specific format constraints.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,4}(\.\d{1,2})?$", ErrorMessage = ErrorMassagePrice)]
        [Comment("The price of the job with up to 6 digits before the decimal point and up to 2 digits after.")]
        public decimal Price { get; set; }
        /// <summary>
        /// Indicates if the job is active (false) or soft deleted (true).
        /// <para>This property helps in managing logical deletion without removing records from the database.</para>
        /// </summary>
        [Comment("Indicates if the job is active or soft deleted.")]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Collection representing the many-to-many relationship between jobs and teams that have completed the job.
        /// </summary>
        [Comment("Collection of mappings between jobs and teams representing job completions.")]
        public virtual IEnumerable<JobDone> JobsDone { get; set; } = new List<JobDone>();

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
