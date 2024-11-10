namespace ElProApp.Web.Models.Building
{
    using System;
    using System.Collections.Generic;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using AutoMapper;

    public class BuildingViewModel : IMapFrom<Building> , IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public ICollection<BuildingTeamMapping> TeamsOnBuilding = new List<BuildingTeamMapping>();

        public ICollection<Guid> selectedTeamEntities = new List<Guid>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Building, BuildingViewModel>()
                .ForMember(b => b.TeamsOnBuilding, x => x.MapFrom(s => s.TeamsOnBuilding));
        }
    }
}
