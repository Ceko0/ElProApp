namespace ElProApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using ElProApp.Services.Data.Interfaces;
    using static Common.EntityValidationConstants.Building;
    using static Common.EntityValidationErrorMessage.Building;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents a building entity with details about its name, location, and associated teams.
    /// </summary>
    public class Building : IDeletableEntity
    {
        /// <summary>
        /// Unique identifier for the building.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Comment("Unique identifier for the building.")]
        public Guid Id { get; set; } = new();

        /// <summary>
        /// The name of the building, with length constraints.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(BuildingNameMinLength, ErrorMessage = ErrorMassageBuildingNameMinLength)]
        [MaxLength(BuildingNameMaxLength, ErrorMessage = ErrorMassageBuildingNameMaxLength)]
        [Comment("The name of the building with a minimum of 3 and a maximum of 50 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The location of the building, with length constraints.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [MinLength(LocationMinLength, ErrorMessage = ErrorMassageLocationMinLength)]
        [MaxLength(LocationMaxLength, ErrorMessage = ErrorMassageLocationMaxLength)]
        [Comment("The location of the building with a minimum of 10 and a maximum of 100 characters.")]
        public string Location { get; set; } = null!;

        /// <summary>
        /// Indicates if the building is active (false) or soft deleted (true).
        /// <para>This property helps in managing logical deletion without removing records from the database.</para>
        /// </summary>
        [Comment("Indicates if the building is active or soft deleted.")]
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
