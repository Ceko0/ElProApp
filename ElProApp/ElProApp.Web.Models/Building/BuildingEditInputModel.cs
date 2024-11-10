
namespace ElProApp.Web.Models.Building
{ 
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using static ElProApp.Common.EntityValidationConstants.Building;
    using static ElProApp.Common.EntityValidationErrorMessage.Building;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;

    public class BuildingEditInputModel : IMapTo<JobDone>, IHaveCustomMappings
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

        public ICollection<Guid> selectedTeamEntities = new List<Guid>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Building, BuildingEditInputModel>()
                .ForMember(b => b.TeamsOnBuilding, b => b.MapFrom(s => s.TeamsOnBuilding));

        }
    }
}
