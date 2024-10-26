namespace ElProApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Models.Mappings;
    using static Common.EntityValidationConstants.Building;
    using static Common.EntityValidationErrorMessage.Building;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents a building entity with details about its name, location, and associated teams.
    /// </summary>
    public class Building
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
        /// Navigation property for the teams working in this building.
        /// </summary>
        [Comment("Collection of teams associated with the building through BuildingTeamMapping.")]
        public virtual ICollection<BuildingTeamMapping> TeamsOnBuilding { get; set; } = new HashSet<BuildingTeamMapping>();
    }
}
