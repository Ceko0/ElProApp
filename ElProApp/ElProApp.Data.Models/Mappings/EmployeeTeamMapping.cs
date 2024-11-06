namespace ElProApp.Data.Models.Mappings
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the many-to-many relationship between Employee and Team.
    /// </summary>
    public class EmployeeTeamMapping
    {
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key for the employee.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Employee entity.")]
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Navigation property for the associated Employee.
        /// </summary>
        [ForeignKey(nameof(EmployeeId))]
        [Comment("Navigation property for the associated Employee.")]
        public Employee Employee { get; set; } = null!;

        /// <summary>
        /// Foreign key for the team.
        /// </summary>
        [Required]
        [Comment("Foreign key referencing the Team entity.")]
        public Guid TeamId { get; set; }

        /// <summary>
        /// Navigation property for the associated Team.
        /// </summary>
        [ForeignKey(nameof(TeamId))]
        [Comment("Navigation property for the associated Team.")]
        public Team Team { get; set; } = null!;
    }
}
