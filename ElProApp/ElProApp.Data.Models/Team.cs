namespace ElProApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Team;
    using static Common.EntityValidationErrorMessage.Team;
    using static Common.EntityValidationErrorMessage.Master;
    using Mappings;

    /// <summary>
    /// Represents a team within the system, including its unique identifier, name, 
    /// and associated relationships to buildings, jobs, and employees.
    /// </summary>
    public class Team 
    {
        /// <summary>
        /// Unique identifier for the team, serving as the primary key.
        /// </summary>
        [Required]
        [Comment("Primary key and unique identifier for the team.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// Name of the team with a specified maximum length.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The name of the team, limited by maximum length.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Indicates if the team is active (false) or soft deleted (true).
        /// <para>This property helps in managing logical deletion without removing records from the database.</para>
        /// </summary>
        [Comment("Indicates if the team is active or soft deleted.")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Collection of mappings representing the buildings assigned to this team.
        /// </summary>
        [Comment("Collection representing the many-to-many relationship between teams and buildings.")]
        public virtual ICollection<BuildingTeamMapping> BuildingWithTeam { get; set; } = [];

        /// <summary>
        /// Collection of mappings representing the jobs completed by this team.
        /// </summary>
        [Comment("Collection representing the many-to-many relationship between teams and completed jobs.")]
        public virtual ICollection<JobDoneTeamMapping> JobsDoneByTeam { get; set; } = [];

        /// <summary>
        /// Collection of mappings representing employees who are members of this team.
        /// </summary>
        [Comment("Collection representing the many-to-many relationship between teams and employees.")]
        public virtual ICollection<EmployeeTeamMapping> EmployeesInTeam { get; set; } = [];
    }
}
