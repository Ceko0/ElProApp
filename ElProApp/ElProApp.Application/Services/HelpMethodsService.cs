namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides helper methods for retrieving user, identity and commonly used application data.
    /// </summary>
    public class HelpMethodsService : IHelpMethodsService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpMethodsService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor.</param>
        /// <param name="userManager">User manager.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public HelpMethodsService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IServiceProvider serviceProvider)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Retrieves the identifier of the currently authenticated user.
        /// </summary>
        /// <returns>The user identifier.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user context is not available.
        /// </exception>
        public string GetUserId()
        {
            var userId = httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException(
                    "Unable to retrieve current user identifier.");

            return userId;
        }

        /// <summary>
        /// Retrieves a user by identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The user.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is not found.
        /// </exception>
        public async Task<IdentityUser> GetUserAsync(string id)
            => await userManager.FindByIdAsync(id)
               ?? throw new InvalidOperationException("User not found.");

        /// <summary>
        /// Converts and validates a string identifier to <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The parsed <see cref="Guid"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the identifier is invalid.
        /// </exception>
        public Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                !Guid.TryParse(id, out Guid validId))
            {
                throw new ArgumentException(
                    "Invalid ID format.", nameof(id));
            }

            return validId;
        }

        /// <summary>
        /// Retrieves the currently authenticated user.
        /// </summary>
        /// <returns>The current user.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the user is not found.
        /// </exception>
        public async Task<IdentityUser> GetCurrentUserAsync()
        {
            var userId = httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not found.");

            return await userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");
        }

        /// <summary>
        /// Retrieves all team mappings for a specific building.
        /// </summary>
        /// <param name="id">The building identifier.</param>
        /// <returns>A collection of mappings.</returns>
        public async Task<List<BuildingTeamMapping>> GetBuildingTeamMapping(Guid id)
        {
            var service =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            return await service
                .GetAllAttached()
                .Include(x => x.Building)
                .Include(x => x.Team)
                .Where(x =>
                    x.BuildingId == id &&
                    !x.Building.IsDeleted &&
                    !x.Team.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves team information associated with a job-done record.
        /// </summary>
        /// <param name="id">The job-done identifier.</param>
        /// <returns>The team.</returns>
        public async Task<Team> GetTeamInforamtion(Guid id)
        {
            var service =
                serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            var mapping = await service
                .GetAllAttached()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.JobDoneId == id);

            return mapping?.Team ?? new Team();
        }

        /// <summary>
        /// Retrieves the roles assigned to a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>A collection of roles.</returns>
        public async Task<IList<string>> GetUserRolesAsync(IdentityUser user)
            => await userManager.GetRolesAsync(user);

        /// <summary>
        /// Retrieves all non-deleted buildings.
        /// </summary>
        /// <returns>Queryable buildings.</returns>
        public IQueryable<Building> GetAllBuildings()
        {
            var service = serviceProvider.GetRequiredService<IBuildingService>();
            return service.GetAllAttached().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Retrieves all non-deleted employees.
        /// </summary>
        /// <returns>Queryable employees.</returns>
        public IQueryable<Employee> GetAllEmployees()
        {
            var service = serviceProvider.GetRequiredService<IEmployeeService>();
            return service.GetAllAttached().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Retrieves all non-deleted job-done records.
        /// </summary>
        /// <returns>Queryable job-done records.</returns>
        public IQueryable<JobDone> GetAllJobDones()
        {
            var service = serviceProvider.GetRequiredService<IJobDoneService>();
            return service.GetAllAttached().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Retrieves all non-deleted jobs (legacy).
        /// </summary>
        /// <returns>Queryable jobs.</returns>
        public IQueryable<Job> GetAllJobs()
        {
            var service = serviceProvider.GetRequiredService<IJobService>();
            return service.GetAllAttached().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Retrieves all non-deleted teams.
        /// </summary>
        /// <returns>Queryable teams.</returns>
        public IQueryable<Team> GetAllTeams()
        {
            var service = serviceProvider.GetRequiredService<ITeamService>();
            return service.GetAllAttached().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Retrieves all building-team mappings.
        /// </summary>
        /// <returns>Queryable mappings.</returns>
        public IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappings()
        {
            var service =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            return service.GetAllAttached();
        }

        /// <summary>
        /// Retrieves all employee-team mappings.
        /// </summary>
        /// <returns>Queryable mappings.</returns>
        public IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMappings()
        {
            var service =
                serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            return service.GetAllAttached();
        }

        /// <summary>
        /// Retrieves all job-done team mappings.
        /// </summary>
        /// <returns>Queryable mappings.</returns>
        public IQueryable<JobDoneTeamMapping> GetAllJobDoneTeamMappings()
        {
            var service =
                serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            return service.GetAllAttached();
        }

        public async Task<Dictionary<Guid, (decimal, decimal)>> GetMaterialWhitQuantityAndPrice(ICollection<JobDoneMaterialMapping> Materials, Guid BuildingId)
        {
         var materialsDict = Materials
            .Where(x => x.Quantity > 0)
            .ToDictionary(x => x.MaterialId, x => x.Quantity);

            var materialsWithPrices = new Dictionary<Guid, (decimal Quantity, decimal Price)>();

            var priceService = serviceProvider.GetRequiredService<IBuildingMaterialPriceService>();

            foreach (var kvp in materialsDict)
            {
                var price = await priceService.GetPriceAsync(BuildingId, kvp.Key) ?? 0m;
                materialsWithPrices[kvp.Key] = (kvp.Value, price);
            } 
        
            return materialsWithPrices;
        }
    }
}