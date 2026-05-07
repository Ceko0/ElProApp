namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models.Mappings;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents material assigned to a building.
    /// </summary>
    public class BuildingMaterialViewModel : IHaveCustomMappings
    {
        /// <summary>
        /// Gets or sets material identifier.
        /// </summary>
        [Required]
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets material name.
        /// </summary>
        [Required]
        public string MaterialName { get; set; } = null!;

        /// <summary>
        /// Gets or sets quantity in the building.
        /// </summary>
        [Required(ErrorMessage = ErrorMassageQuantity)]
        [Range(0, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        [Required]
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets building name.
        /// </summary>
        public string? BuildingName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<BuildingMaterialMapping, BuildingMaterialViewModel>()
                .ForMember(d => d.MaterialId,
                    opt => opt.MapFrom(s => s.MaterialId))
                .ForMember(d => d.MaterialName,
                    opt => opt.MapFrom(s => s.Material.Name))
                .ForMember(d => d.Quantity,
                    opt => opt.MapFrom(s => s.Quantity))
                .ForMember(d => d.BuildingId,
                    opt => opt.MapFrom(s => s.BuildingId))
                .ForMember(d => d.BuildingName,
                    opt => opt.MapFrom(s => s.Building.Name));
        }
    }
}