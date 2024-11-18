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
        public Guid Id { get; set; } = Guid.NewGuid() ;

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
    }
}
