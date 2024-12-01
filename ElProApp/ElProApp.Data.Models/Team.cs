namespace ElProApp.Data.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Team;
    using static Common.EntityValidationErrorMessage.Team;
    using static Common.EntityValidationErrorMessage.Master;
    using ElProApp.Services.Data.Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;


    /// <summary>
    /// Represents a team within the system, including its unique identifier, name, 
    /// and associated relationships to buildings, jobs, and employees.
    /// </summary>
    public class Team : IDeletableEntity
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
