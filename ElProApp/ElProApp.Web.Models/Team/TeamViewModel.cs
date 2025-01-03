namespace ElProApp.Web.Models.Team
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;

    public class TeamViewModel : IMapFrom<Team>
    {
        public Guid Id { get; set; }
        // Name of the team
        public string Name { get; set; } = null!;

        // Collection of Buildings associated with the team
        public ICollection<BuildingTeamMapping> BuildingWithTeam { get; set; } = new List<BuildingTeamMapping>();

        // Collection of JobsList completed by the team
        public ICollection<JobDoneTeamMapping> JobsDoneByTeam { get; set; } = new List<JobDoneTeamMapping>();

        // Collection of employees who are part of the team
        public ICollection<EmployeeTeamMapping> EmployeesInTeam { get; set; } = new List<EmployeeTeamMapping>();

    }
}
