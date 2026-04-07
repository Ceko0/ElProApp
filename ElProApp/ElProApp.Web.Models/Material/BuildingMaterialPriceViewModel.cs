namespace ElProApp.Web.Models.Material
{
    using System;

    /// <summary>
    /// View model for displaying material prices per building.
    /// </summary>
    public class BuildingMaterialPriceViewModel
    {
        /// <summary>
        /// Gets or sets the building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets the building name.
        /// </summary>
        public string BuildingName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the material identifier.
        /// </summary>
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }
    }
}