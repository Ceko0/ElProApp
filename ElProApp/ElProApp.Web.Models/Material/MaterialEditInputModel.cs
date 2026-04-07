namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;

    using static Common.EntityValidationConstants.Material;
    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents input model used for editing a material.
    /// </summary>
    public class MaterialEditInputModel : IMapFrom<Material>, IMapTo<Material>
    {
        /// <summary>
        /// Gets or sets material identifier.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets material name.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}