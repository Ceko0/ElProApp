namespace ElProApp.Web.Models.Building
{
    using System;
    using System.Collections.Generic;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Material;

    /// <summary>
    /// Represents a building with its related data for visualization.
    /// </summary>
    public class BuildingViewModel : IMapFrom<Building>
    {
        /// <summary>
        /// Gets or sets the building identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the building name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the building location.
        /// </summary>
        public string Location { get; set; } = null!;

        /// <summary>
        /// Gets or sets the teams assigned to the building.
        /// </summary>
        public ICollection<BuildingTeamMapping> TeamsOnBuilding { get; set; }
            = new List<BuildingTeamMapping>();

        /// <summary>
        /// Gets or sets selected team identifiers (used in edit scenarios).
        /// </summary>
        public ICollection<Guid> SelectedTeamEntities { get; set; }
            = new List<Guid>();

        /// <summary>
        /// Gets or sets the materials assigned to the building.
        /// </summary>
        public ICollection<BuildingMaterialViewModel> Materials { get; set; }
            = new List<BuildingMaterialViewModel>();
    }
}