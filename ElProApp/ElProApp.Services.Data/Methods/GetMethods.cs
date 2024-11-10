namespace ElProApp.Services.Data.Methods
{
    using Microsoft.AspNetCore.Http;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;

    public class GetMethods(IBuildingService _buildingService
        , IEmployeeTeamMappingService _employeeTeamMpaping
        , IBuildingTeamMappingService _buildingTeamMappingService
        , IJobDoneTeamMappingService _jobDoneTeamMappingService
        , IHttpContextAccessor _httpContextAccessor
        , IEmployeeService _employeeService
        , IJobDoneService _jobDoneService
        ,ITeamService _teamService
        ,IJobService _jobService)        

    {
        private readonly IHttpContextAccessor httpContextAccessor = _httpContextAccessor;
        private readonly IBuildingService buildingService = _buildingService;
        private readonly IBuildingTeamMappingService buildingTeamMappingService = _buildingTeamMappingService;
        private readonly IEmployeeTeamMappingService employeeTeamMappingService = _employeeTeamMpaping;
        private readonly IJobDoneTeamMappingService jobDoneTeamMappingService = _jobDoneTeamMappingService;
        private readonly IEmployeeService employeeService = _employeeService;
        private readonly IJobDoneService jobDoneService = _jobDoneService;
        private readonly ITeamService teamService = _teamService;
        private readonly IJobService jobService = _jobService;

        public IQueryable<Employee> GetAllEmployeesAttached() =>  employeeService.GetAllAttached();        
        public IQueryable<Team> GetAllTeamsAttached() =>  teamService.GetAllAttached();
        public IQueryable<Building> GetAllBuildingAttached() => buildingService.GetAllAttached();
        public IQueryable<JobDone> GetAllJobDoneAttached() =>  jobDoneService.GetAllAttached();
        public IQueryable<Job> GetAllJobAttached() =>  jobService.GetAllAttached();
        public IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappingAttached()
            =>  buildingTeamMappingService.GetAllAttached();
        public IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMappingAttached()
            =>  employeeTeamMappingService.GetAllAttached();

    }


}
