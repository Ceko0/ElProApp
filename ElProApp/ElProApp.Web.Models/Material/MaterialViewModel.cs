namespace ElProApp.Web.Models.Material
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using AutoMapper;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;

    using static Common.EntityValidationErrorMessage.Master;

    /// <summary>
    /// Represents view model for material visualization.
    /// </summary>
    public class MaterialViewModel : IMapFrom<Material>, IHaveCustomMappings
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets materials per buildings.
        /// </summary>
        public ICollection<BuildingMaterialViewModel> BuildingMaterials { get; set; }
            = new List<BuildingMaterialViewModel>();

        public IEnumerable<SelectListItem> Buildings { get; set; }
            = new List<SelectListItem>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Material, MaterialViewModel>()
                .ForMember(dest => dest.Buildings,
                    opt => opt.Ignore());
        }
    }
}