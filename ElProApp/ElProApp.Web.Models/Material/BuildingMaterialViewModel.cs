namespace ElProApp.Web.Models.Material
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models.Mappings;
    using AutoMapper;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;


    public class BuildingMaterialViewModel : IHaveCustomMappings
    {
        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        public string MaterialName { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageQuantity)]
        [Range(0, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        public decimal Quantity { get; set; }

        [Required]
        public Guid BuildingId { get; set; }

        [Required]
        public string BuildingName { get; set; } = null!;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<BuildingMaterialMapping, BuildingMaterialViewModel>()
                .ForMember(d => d.MaterialId,
                    opt => opt.MapFrom(s => s.Material.Id))
                .ForMember(d => d.MaterialName,
                    opt => opt.MapFrom(s => s.Material.Name))
                .ForMember(d => d.Quantity,
                    opt => opt.MapFrom(s => s.Quantity))
                .ForMember(d => d.BuildingId,
                    opt => opt.MapFrom(s => s.Building.Id))
                .ForMember(d => d.BuildingName,
                    opt => opt.MapFrom(s => s.Building.Name));
        }
    }
}
