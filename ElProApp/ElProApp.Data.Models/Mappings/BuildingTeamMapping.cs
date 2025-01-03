﻿namespace ElProApp.Data.Models.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the many-to-many relationship between Building and Team.
    /// </summary>
    public class BuildingTeamMapping
    {
        /// <summary>
        /// Foreign key referencing the related Building.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Building entity.")]
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Navigation property for the associated Building.
        /// </summary>
        [ForeignKey(nameof(BuildingId))]
        [Comment("Navigation property for the Building associated with this mapping.")]
        public Building Building { get; set; } = null!;

        /// <summary>
        /// Foreign key referencing the related Team.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Team entity.")]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Navigation property for the associated Team.
        /// </summary>
        [ForeignKey(nameof(TeamId))]
        [Comment("Navigation property for the Team associated with this mapping.")]
        public Team Team { get; set; } = null!;

        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
    }
}
