namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Web.Models.Team;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides application-level operations for managing teams.
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly IRepository<Team, Guid> teamRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamService"/> class.
        /// </summary>
        public TeamService(
            IRepository<Team, Guid> teamRepository,
            IServiceProvider serviceProvider,
            IHelpMethodsService helpMethodsService)
        {
            this.teamRepository = teamRepository;
            this.serviceProvider = serviceProvider;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Prepares an input model for creating a new team.
        /// </summary>
        public async Task<TeamInputModel> AddAsync()
        {
            return new TeamInputModel
            {
                BuildingWithTeam = await helpMethodsService
                    .GetAllBuildings()
                    .Where(x => !x.IsDeleted)
                    .To<BuildingViewModel>()
                    .ToListAsync(),

                AvailableEmployees = await helpMethodsService
                    .GetAllEmployees()
                    .Where(x => !x.IsDeleted)
                    .To<EmployeeViewModel>()
                    .ToListAsync(),

                JobsDoneByTeam = await helpMethodsService
                    .GetAllJobDones()
                    .Where(x => !x.IsDeleted)
                    .To<JobDoneViewModel>()
                    .ToListAsync()
            };
        }

        /// <summary>
        /// Creates a new team.
        /// </summary>
        public async Task<string> AddAsync(TeamInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            bool exists = await teamRepository
                .GetAllAttached()
                .AnyAsync(x => x.Name == model.Name && !x.IsDeleted);

            if (exists)
                throw new InvalidOperationException(
                    "A team with the same name already exists.");

            var team = AutoMapperConfig.MapperInstance.Map<Team>(model);
            await teamRepository.AddAsync(team);

            var buildingTeamMappingService =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            var employeeTeamMappingService =
                serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            if (model.SelectedBuildingId.HasValue)
            {
                await buildingTeamMappingService.AddAsync(
                    model.SelectedBuildingId.Value,
                    team.Id);
            }


            foreach (var employeeId in model.SelectedEmployeeIds.Distinct())
            {
                await employeeTeamMappingService
                    .AddAsync(employeeId, team.Id);
            }

            return team.Id.ToString();
        }

        /// <summary>
        /// Retrieves a team edit model by identifier.
        /// </summary>
        public async Task<TeamEditInputModel> EditByIdAsync(string id)
        {
            Guid teamId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var team = await teamRepository.GetByIdAsync(teamId)
                ?? throw new InvalidOperationException("Team not found.");

            if (team.IsDeleted)
                throw new InvalidOperationException("Team is deleted.");

            await EnsureUserHasAccessAsync(teamId);

            return new TeamEditInputModel
            {
                Id = team.Id,
                Name = team.Name,
                BuildingWithTeam = await helpMethodsService
                    .GetAllBuildings()
                    .To<BuildingViewModel>()
                    .ToListAsync(),

                JobsDoneByTeam = await helpMethodsService
                    .GetAllJobDones()
                    .To<JobDoneViewModel>()
                    .ToListAsync(),

                EmployeesInTeam = await helpMethodsService
                    .GetAllEmployees()
                    .To<EmployeeViewModel>()
                    .ToListAsync(),

                BuildingWithTeamIds = await helpMethodsService
                    .GetAllBuildingTeamMappings()
                    .Where(x => x.TeamId == teamId)
                    .Select(x => x.BuildingId)
                    .ToListAsync(),

                JobsDoneByTeamIds = await helpMethodsService
                    .GetAllJobDoneTeamMappings()
                    .Where(x => x.TeamId == teamId)
                    .Select(x => x.JobDoneId)
                    .ToListAsync(),

                EmployeesInTeamIds = await helpMethodsService
                    .GetAllEmployeeTeamMappings()
                    .Where(x => x.TeamId == teamId)
                    .Select(x => x.EmployeeId)
                    .ToListAsync()
            };
        }

        /// <summary>
        /// Updates a team.
        /// </summary>
        public async Task<bool> EditByModelAsync(TeamEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await EnsureUserHasAccessAsync(model.Id);

            var team = await teamRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("Team not found.");

            AutoMapperConfig.MapperInstance.Map(model, team);
            await teamRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// Retrieves all non-deleted teams.
        /// </summary>
        public async Task<ICollection<TeamViewModel>> GetAllAsync()
            => await teamRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<TeamViewModel>()
                .ToListAsync();

        /// <summary>
        /// Retrieves all attached teams.
        /// </summary>
        public IQueryable<Team> GetAllAttached()
            => teamRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a team by identifier.
        /// </summary>
        public async Task<TeamViewModel> GetByIdAsync(string id)
        {
            Guid teamId = helpMethodsService.ConvertAndTestIdToGuid(id);
            await EnsureUserHasAccessAsync(teamId);

            return await teamRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<TeamViewModel>()
                .FirstOrDefaultAsync(x => x.Id == teamId)
                ?? throw new InvalidOperationException("Team not found.");
        }

        /// <summary>
        /// Soft deletes a team.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid teamId = helpMethodsService.ConvertAndTestIdToGuid(id);
            return await teamRepository.SoftDeleteAsync(teamId);
        }

        /// <summary>
        /// Checks whether a team exists.
        /// </summary>
        public async Task<bool> Any(Guid id)
        {
            var team = await teamRepository.GetByIdAsync(id);
            return team != null && !team.IsDeleted;
        }

        /// <summary>
        /// Ensures the current user has access to the specified team.
        /// </summary>
        private async Task EnsureUserHasAccessAsync(Guid teamId)
        {
            var userId = helpMethodsService.GetUserId();

            var mapping = await helpMethodsService
                .GetAllEmployeeTeamMappings()
                .FirstOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.Employee.UserId == userId);

            var user = await helpMethodsService.GetUserAsync(userId);
            var roles = await helpMethodsService.GetUserRolesAsync(user);

            if (!roles.Contains("Admin") && mapping == null)
                throw new UnauthorizedAccessException();
        }
    }
}
