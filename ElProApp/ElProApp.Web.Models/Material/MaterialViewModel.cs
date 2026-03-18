namespace ElProApp.Web.Models.Material
{ 
    using System;
    using System.ComponentModel.DataAnnotations;
    
    using Microsoft.AspNetCore.Mvc.Rendering;
    
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;

    using static Common.EntityValidationErrorMessage.Material;
    using static Common.EntityValidationErrorMessage.Master;
    using AutoMapper;

    public class MaterialViewModel : IMapFrom<Material>, IHaveCustomMappings
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        public string Name { get; set; } = null!;

        [Range(0, 999999.99, ErrorMessage = ErrorMassagePozitive)]
        public decimal Quantity { get; set; }

        public Guid BuildingId { get; set; }

        public string? BuildingName { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();

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
