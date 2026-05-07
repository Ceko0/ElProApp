namespace ElProApp.Web.Models.Material
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Input model for setting material price per building.
    /// </summary>
    public class BuildingMaterialPriceInputModel
    {
        /// <summary>
        /// Gets or sets the building identifier.
        /// </summary>
        [Required]
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets the material identifier.
        /// </summary>
        [Required]
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets available buildings.
        /// </summary>
        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets available materials.
        /// </summary>
        public IEnumerable<SelectListItem> Materials { get; set; }
            = new List<SelectListItem>();
    }
}
