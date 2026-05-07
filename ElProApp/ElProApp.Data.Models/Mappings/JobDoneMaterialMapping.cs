namespace ElProApp.Data.Models.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the many-to-many relationship between JobDone and Material,
    /// including the quantity of material used.
    /// </summary>
    public class JobDoneMaterialMapping
    {
        /// <summary>
        /// Foreign key referencing the related JobDone.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the JobDone entity.")]
        public Guid JobDoneId { get; set; }

        /// <summary>
        /// Navigation property for the associated JobDone.
        /// </summary>
        [ForeignKey(nameof(JobDoneId))]
        [Comment("Navigation property for the JobDone associated with this mapping.")]
        public JobDone JobDone { get; set; } = null!;

        /// <summary>
        /// Foreign key referencing the related Material.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Material entity.")]
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Navigation property for the associated Material.
        /// </summary>
        [ForeignKey(nameof(MaterialId))]
        [Comment("Navigation property for the Material associated with this mapping.")]
        public Material Material { get; set; } = null!;

        /// <summary>
        /// Quantity of material used for the job.
        /// </summary>
        [Required]
        [Comment("Quantity of material used for the job.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Snapshot of the material unit price at the time the job was done.
        /// </summary>
        [Required]
        [Comment("Snapshot of material unit price at the time of job execution.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }


        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]

        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;

        public bool IsDeleted { get; set; }

        public DateTime DeletedOn { get; set; }

    }
}
