namespace ElProApp.Web.Models.Team
{
    using AutoMapper;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;

    public class TeamViewModel : IMapFrom<Team> , IHaveCustomMappings
    {
        public Guid Id { get; set; }
        // Name of the team
        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        // Collection of buildings associated with the team
        public ICollection<BuildingTeamMapping> BuildingWithTeam { get; set; } = new List<BuildingTeamMapping>();

        // Collection of jobs completed by the team
        public ICollection<JobDoneTeamMapping> JobsDoneByTeam { get; set; } = new List<JobDoneTeamMapping>();

        // Collection of employees who are part of the team
        public ICollection<EmployeeTeamMapping> EmployeesInTeam { get; set; } = new List<EmployeeTeamMapping>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Team, TeamViewModel>()
                .ForMember(t => t.BuildingWithTeam, x => x.MapFrom(s => s.BuildingWithTeam));
            configuration.CreateMap<Team, TeamViewModel>()
                .ForMember(t => t.JobsDoneByTeam, x => x.MapFrom(s => s.JobsDoneByTeam));
            configuration.CreateMap<Team, TeamViewModel>()
                .ForMember(t => t.EmployeesInTeam , x => x.MapFrom(s => s.EmployeesInTeam));
        }
    }
}
