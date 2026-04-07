namespace ElProApp.Web.Models.Material
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Represents input model used for assigning a material to a building with quantity.
    /// </summary>
    public class BuildingMaterialInputModel
    {
        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        [Required(ErrorMessage = "Сградата е задължителна.")]
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets material identifier.
        /// </summary>
        [Required(ErrorMessage = "Материалът е задължителен.")]
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets quantity to assign.
        /// </summary>
        [Required(ErrorMessage = "Количество е задължително.")]
        [Range(0.01, 100000, ErrorMessage = "Количество трябва да е по-голямо от 0.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets available buildings for selection.
        /// </summary>
        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets available materials for selection.
        /// </summary>
        public IEnumerable<SelectListItem> Materials { get; set; }
            = new List<SelectListItem>();
    }
}