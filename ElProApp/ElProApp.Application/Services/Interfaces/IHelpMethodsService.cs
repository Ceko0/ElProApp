namespace ElProApp.Application.Services.Interfaces
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    
    using ElProApp.Data.Models;
using ElProApp.Data.Models.Mappings;

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

        IQueryable<Team> GetAllTeams();

        IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappings();

        IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMappings();

        IQueryable<JobDoneTeamMapping> GetAllJobDoneTeamMappings();
        Task<Dictionary<Guid, (decimal, decimal)>> GetMaterialWhitQuantityAndPrice(ICollection<JobDoneMaterialMapping> Materials, Guid BuildingId);
        Employee GetEmployeeByUserId(string? id);
    }
}
