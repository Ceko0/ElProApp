namespace ElProApp.Web.Models.Building
{
    using System;
    using System.Collections.Generic;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using AutoMapper;

    public class BuildingViewModel : IMapFrom<Building>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public ICollection<BuildingTeamMapping> TeamsOnBuilding = new List<BuildingTeamMapping>();

        public ICollection<Guid> selectedTeamEntities = new List<Guid>();

       
    }
}
