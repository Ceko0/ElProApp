namespace ElProApp.Data.Models.Mappings
{
    using System;

    /// <summary>
    /// Represents a price definition for a material within a specific building.
    /// </summary>
    public class BuildingMaterialPrice
    {
        /// <summary>
        /// Gets or sets the building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets the building.
        /// </summary>
        public virtual Building Building { get; set; } = null!;

        /// <summary>
        /// Gets or sets the material identifier.
        /// </summary>
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public virtual Material Material { get; set; } = null!;

        /// <summary>
        /// Gets or sets the price for the material in the building.
        /// </summary>
        public decimal Price { get; set; }
    }
}