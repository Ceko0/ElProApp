namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using AutoMapper;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents view model for material visualization.
    /// </summary>
    public class MaterialViewModel : IMapFrom<Material>, IHaveCustomMappings
    {
        /// <summary>
        /// Gets or sets material identifier.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets material name.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets material quantity.
        /// </summary>
        [Range(0, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets building name.
        /// </summary>
        public string? BuildingName { get; set; }

        /// <summary>
        /// Gets or sets material price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets available buildings for selection.
        /// </summary>
        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets building-material quantities for the material.
        /// </summary>
        public ICollection<BuildingMaterialViewModel> BuildingMaterials { get; set; }
            = new List<BuildingMaterialViewModel>();

        /// <summary>
        /// Configures custom mappings.
        /// </summary>
        /// <param name="configuration">AutoMapper configuration.</param>
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Material, MaterialViewModel>()
                .ForMember(dest => dest.BuildingName,
                    opt => opt.MapFrom(src =>
                        src.Buildings
                            .Select(x => x.Building.Name)
                            .FirstOrDefault()))
                .ForMember(dest => dest.Buildings,
                    opt => opt.Ignore());
        }


    }
}