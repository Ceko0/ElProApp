namespace ElProApp.Services.Data
{ 
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Models;
    using Interfaces;

    public class HelpMethodsService(IHttpContextAccessor httpContextAccessor,
                                    UserManager<IdentityUser> userManager,
                                    IServiceProvider serviceProvider):
                                    IHelpMethodsService
    {

        /// <summary>
        /// Retrieves the ID of the currently logged-in user from their claims.
        /// </summary>
        /// <returns>The user ID.</returns>
        public string GetUserId()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("Failed to retrieve UserId. Please try again.");
            return userId;
        }

        /// <summary>
        /// Retrieves the user associated with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The <see cref="IdentityUser"/> object associated with the given ID.</returns>
        public async Task<IdentityUser> GetUserAsync(string id)
            => await userManager.FindByIdAsync(id)
               ?? throw new InvalidOperationException("Invalid user.");

        /// <summary>
        /// Converts and validates the provided ID to a valid <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The ID to validate and convert.</param>
        /// <returns>The converted <see cref="Guid"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the ID format is invalid.</exception>
        public  Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        /// <summary>
        /// Retrieves the currently logged-in user through UserManager if no employee-specific UserId is set.
        /// </summary>
        /// <returns>The <see cref="IdentityUser"/> representing the current user.</returns>
        public async Task<IdentityUser> GetCurrentUserAsync()
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new InvalidOperationException("User not found.");

            return await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException("Invalid user.");
        }

        /// <summary>
        /// Retrieves all team mappings for a building.
        /// </summary>
        /// <param name="id">The ID of the building.</param>
        /// <returns>A list of <see cref="BuildingTeamMapping"/> objects associated with the building.</returns>
        public async Task<List<BuildingTeamMapping>> GetBuildingTeamMapping(Guid id)
        {
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            return await buildingTeamMappingService
                .GetAllAttached()
                .Include(x => x.Building)
                .Include(x => x.Team)
                .Where(x => x.BuildingId == id && !x.Building.IsDeleted && !x.Team.IsDeleted).ToListAsync();
        }

        public async Task<Team> GetTeamInforamtion(Guid id)
        {
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var allTeamMappings = await jobDoneTeamMappingService
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.JobDone)
                .ToListAsync();

            var currentTeam = allTeamMappings.FirstOrDefault(x => x.JobDoneId == id);
            Team result = new Team();

            if (currentTeam != null) result = currentTeam.Team;
            return result;
        }

        public async Task<IList<string>> GetUserRolesAsync(IdentityUser user) => await userManager.GetRolesAsync(user);

        /// <summary>
        /// Retrieves all Buildings attached to the service.
        /// </summary>
        public IQueryable<Building> GetAllBuildings()
        {
            var service = serviceProvider.GetRequiredService<IBuildingService>();
            var model = service.GetAllAttached().Where(x => !x.IsDeleted);
            return model;
        }

        /// <summary>
        /// Retrieves all employees attached to the service.
        /// </summary>
        public IQueryable<Employee> GetAllEmployees()
        {
            var service = serviceProvider.GetRequiredService<IEmployeeService>();
            var model = service.GetAllAttached().Where(x => !x.IsDeleted);
            return model;
        }

        /// <summary>
        /// Retrieves all job done records attached to the service.
        /// </summary>
        public IQueryable<JobDone> GetAllJobDones()
        {
            var service = serviceProvider.GetRequiredService<IJobDoneService>();
            var model = service.GetAllAttached().Where(x => !x.IsDeleted);
            return model;
        }

        /// <summary>
        /// Retrieves all JobsList attached to the service.
        /// </summary>
        public IQueryable<Job> GetAllJobs()
        {
            var service = serviceProvider.GetRequiredService<IJobService>();
            var model = service.GetAllAttached().Where(x => !x.IsDeleted);
            return model;
        }

        public IQueryable<Team> GetAllTeam()
        {
            var service = serviceProvider.GetRequiredService<ITeamService>();
            var model = service.GetAllAttached().Where(x => !x.IsDeleted);
            return model;
        }

        /// <summary>
        /// Retrieves all building-team mappings attached to the service.
        /// </summary>
        public IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappings()
        {
            var service = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var model = service.GetAllAttached();
            return model;
        }

        /// <summary>
        /// Retrieves all employee-team mappings attached to the service.
        /// </summary>
        public IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMаppings()
        {
            var service = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var model = service.GetAllAttached();
            return model;
        }

        /// <summary>
        /// Retrieves all job done-team mappings attached to the service.
        /// </summary>
        public IQueryable<JobDoneTeamMapping> GetAllJobDoneTeamMappings()
        {
            var service = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var model = service.GetAllAttached();
            return model;
        }

    }
}
