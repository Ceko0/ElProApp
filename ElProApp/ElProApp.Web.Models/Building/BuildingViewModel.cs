namespace ElProApp.Web.Models.Building
{
    using System;
    using System.Collections.Generic;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Material;

    public class BuildingViewModel : IMapFrom<Building>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public ICollection<BuildingTeamMapping> TeamsOnBuilding = new List<BuildingTeamMapping>();

        public ICollection<Guid> selectedTeamEntities = new List<Guid>();

        public ICollection<BuildingMaterialViewModel> Materials { get; set; } = new List<BuildingMaterialViewModel>();
    }
}
