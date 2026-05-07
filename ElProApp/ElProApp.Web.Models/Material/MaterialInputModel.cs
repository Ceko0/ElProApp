namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;

    using static Common.EntityValidationConstants.Material;
    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents input model used for creating a material.
    /// </summary>
    public class MaterialInputModel : IMapTo<Material>
    {
        /// <summary>
        /// Gets or sets material name.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;


        /// <summary>
        /// Gets or sets available buildings for selection.
        /// </summary>
        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();
    }
}