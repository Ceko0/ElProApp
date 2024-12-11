using ElProApp.Data.Models;
using ElProApp.Data.Models.Mappings;

namespace ElProApp.Services.Data.Interfaces
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public interface IHelpMethodsService
    {
        string GetUserId();

        Task<IdentityUser> GetUserAsync(string id);

        Guid ConvertAndTestIdToGuid(string id);

        Task<IdentityUser> GetCurrentUserAsync();

        Task<Team> GetTeamInforamtion(Guid id);

        Task<List<BuildingTeamMapping>> GetBuildingTeamMapping(Guid id);

        Task<IList<string>> GetUserRolesAsync(IdentityUser user);

        IQueryable<Building> GetAllBuildings();

        IQueryable<Employee> GetAllEmployees();

        IQueryable<JobDone> GetAllJobDones();

        IQueryable<Job> GetAllJobs();

        IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappings();

        IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMаppings();

        IQueryable<JobDoneTeamMapping> GetAllJobDoneTeamMappings();
    }
}
