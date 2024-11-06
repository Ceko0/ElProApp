namespace ElProApp.Web.Models.Building
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using static Common.EntityValidationConstants.Building;
    using static Common.EntityValidationErrorMessage.Building;
    using static Common.EntityValidationErrorMessage.Master;
    using AutoMapper;
    using ElProApp.Web.Models.Employee;

    public class BuildingViewModel : IMapFrom<Building> , IHaveCustomMappings
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(BuildingNameMinLength, ErrorMessage = ErrorMassageBuildingNameMinLength)]
        [MaxLength(BuildingNameMaxLength, ErrorMessage = ErrorMassageBuildingNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(LocationMinLength, ErrorMessage = ErrorMassageLocationMinLength)]
        [MaxLength(LocationMaxLength, ErrorMessage = ErrorMassageLocationMaxLength)]
        public string Location { get; set; } = null!;

        public ICollection<BuildingTeamMapping> TeamsOnBuilding = new List<BuildingTeamMapping>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Building, BuildingViewModel>()
                .ForMember(b => b.TeamsOnBuilding, x => x.MapFrom(s => s.TeamsOnBuilding));
        }
    }
}
