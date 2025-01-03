﻿namespace ElProApp.Data.Models.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the many-to-many relationship between JobDone and Team.
    /// </summary>
    public class JobDoneTeamMapping
    {
        /// <summary>
        /// Foreign key for the job done.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the JobDone entity.")]
        public Guid JobDoneId { get; set; }

        /// <summary>
        /// Navigation property for the associated JobDone entity.
        /// </summary>
        [ForeignKey(nameof(JobDoneId))]
        [Comment("Navigation property for the associated JobDone.")]
        public JobDone JobDone { get; set; } = null!;

        /// <summary>
        /// Foreign key for the team.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Team entity.")]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Navigation property for the associated Team entity.
        /// </summary>
        [ForeignKey(nameof(TeamId))]
        [Comment("Navigation property for the associated Team.")]
        public Team Team { get; set; } = null!;

        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
    }
}
