namespace ElProApp.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents the many-to-many relationship between Building and Material,
    /// including the quantity of material used.
    /// </summary>
    public class BuildingMaterialMapping
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
        /// Quantity of material used at the Building.
        /// </summary>
        [Comment("Quantity of material used at the Building.")]
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        [Range(0.01, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        [RegularExpression(@"^\d{1,6}(\.\d{1,2})?$", ErrorMessage = ErrorMassageQuantity)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The date when the record was created.
        /// </summary>
        [Comment("The date when the record was created.")]
        [Column(TypeName = "date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now.Date;
        public DateTime LastUpdated { get; set; }
    }
}
