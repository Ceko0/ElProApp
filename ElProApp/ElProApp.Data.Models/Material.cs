namespace ElProApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidationConstants.Material;
    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;
    using System.ComponentModel.DataAnnotations.Schema;
    using ElProApp.Services.Data.Interfaces;

    /// <summary>
    /// Represents a material entity used in the system.
    /// Contains metadata about the material such as name, quantity, creation and deletion state.
    /// </summary>
    public class Material : IDeletableEntity
    {
        /// <summary>
        /// Unique identifier for the material.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Comment("Unique identifier for the material.")]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the material with a maximum of 50 characters.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        [Comment("The name of the material with a maximum of 50 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The quantity of the material.
        /// Value must be positive and formatted with up to two decimal places.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 9999.99, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        [Comment("The quantity of the material.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Indicates whether the material is active (false) or soft-deleted (true).
        /// </summary>
        [Comment("Indicates if the material is active or soft deleted.")]
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

        [ForeignKey(nameof(Building))]
        public Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;
    }
}
