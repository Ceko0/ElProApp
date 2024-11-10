namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Web.Models.Team;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class TeamService(IRepository<Team, Guid> _teamRepository
        , IBuildingService _buildingService
        , IEmployeeTeamMappingService _employeeTeamMpaping
        , IBuildingTeamMappingService _buildingTeamMappingService
        , IJobDoneTeamMappingService _jobDoneTeamMappingService
        , IHttpContextAccessor _httpContextAccessor
        , IEmployeeService _employeeService
        , IJobDoneService _jobDoneService)
        : ITeamService
    {
        private readonly IHttpContextAccessor httpContextAccessor = _httpContextAccessor;
        private readonly IRepository<Team, Guid> teamRepository = _teamRepository;
        private readonly IBuildingService buildingService = _buildingService;
        private readonly IBuildingTeamMappingService buildingTeamMappingService = _buildingTeamMappingService;
        private readonly IEmployeeTeamMappingService employeeTeamMappingService = _employeeTeamMpaping;
        private readonly IJobDoneTeamMappingService jobDoneTeamMappingService = _jobDoneTeamMappingService;
        private readonly IEmployeeService employeeService = _employeeService;
        private readonly IJobDoneService jobDoneService = _jobDoneService;

        public async Task<TeamInputModel> AddAsync()
        {
            var model = new TeamInputModel()
            {
                BuildingWithTeam = await buildingService.GetAllAsync(),

                AvailableEmployees = await employeeService.GetAllAsync(),

                JobsDoneByTeam = await jobDoneService.GetAllAsync(),
            };

            return model;
        }

        /// <summary>
        /// Adds a new team based on the provided input model. 
        /// Throws an exception if a team with the same name already exists.
        /// </summary>
        /// <param name="model">TeamAddInputModel containing data for the new team.</param>
        /// <returns>ID of the newly created team as a string.</returns>
        public async Task<string> AddAsync(TeamInputModel model)
        {
            if (await teamRepository.FirstOrDefaultAsync(x => x.Name == model.Name) != null)
                throw new InvalidOperationException("A team with this name already exists!");

            var team = AutoMapperConfig.MapperInstance.Map<Team>(model);

            if (model.SelectedBuildingId != Guid.Empty)
            {
                BuildingViewModel building = buildingService.GetById(model.SelectedBuildingId.ToString()!) 
                    ?? throw new InvalidOperationException("The selected building does not exist.");
                await teamRepository.AddAsync(team);
                string stringUserId = GetUserId();
                if (!Guid.TryParse(stringUserId, out Guid userId)) throw new ArgumentException("Invalid user id");

                var employeEntity = employeeService.GetByUserId(stringUserId);

                var employeeTeamMapping = await employeeTeamMappingService.AddAsync(employeEntity.Id, team.Id);

                var buildingTeamMapping = await buildingTeamMappingService.AddAsync(building.Id, team.Id);

                _ = building.TeamsOnBuilding.Append(buildingTeamMapping);
                _ = team.BuildingWithTeam.Append(buildingTeamMapping);

                _ = employeEntity.TeamsEmployeeBelongsTo.Append(employeeTeamMapping);
                _ = team.EmployeesInTeam.Append(employeeTeamMapping);

                foreach (var employeeId in model.SelectedEmployeeIds)
                {
                    var employeeMapping = await employeeTeamMappingService.AddAsync(employeeId, team.Id);
                    var employee = await employeeService.GetByIdAsync(employeeId.ToString());
                    employee?.TeamsEmployeeBelongsTo.Append(employeeMapping);
                    _ = team.EmployeesInTeam.Append(employeeTeamMapping);
                }
            }

            return team.Id.ToString();
        }

        /// <summary>
        /// Retrieves a team by its ID for editing purposes. 
        /// Throws an exception if the team is marked as deleted.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamEditInputModel mapped from the retrieved team.</returns>
        public async Task<TeamEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            Team entity = await teamRepository.GetByIdAsync(validId);
            if (entity.IsDeleted) throw new InvalidOperationException("Team is deleted.");
            var userId = GetUserId();
            var allEmployeeTeamMapping = await employeeTeamMappingService.GetByTeamIdAsync(entity.Id);

            var employeeTeamMapping = new List<EmployeeTeamMapping>(allEmployeeTeamMapping.ToList().Where(x => x.Employee.UserId == userId))
                ?? throw new AccessViolationException("User does not have permission to edit this team.");
            var model = new TeamEditInputModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingWithTeam = new List<BuildingViewModel>(await buildingService.GetAllAsync()),
                JobsDoneByTeam = new List<JobDoneViewModel>(await jobDoneService.GetAllAsync()),
                EmployeesInTeam = new List<EmployeeViewModel>(await employeeService.GetAllAsync()),
                BuildingWithTeamIds = new List<Guid>((await buildingTeamMappingService.GetByTeamIdAsync(entity.Id)).Select(x => x.BuildingId)),
                JobsDoneByTeamIds = new List<Guid>((await jobDoneTeamMappingService.GetByTeamIdAsync(entity.Id)).Select(x => x.JobDoneId)),
                EmployeesInTeamIds = new List<Guid>((await employeeTeamMappingService.GetByTeamIdAsync(entity.Id)).Select(x => x.EmployeeId))
            };


            return model;
        }

        /// <summary>
        /// Updates a team entity based on the provided input model.
        /// Returns true if the update was successful, otherwise false.
        /// </summary>
        /// <param name="model">TeamEditInputModel containing updated team data.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> EditByModelAsync(TeamEditInputModel model)
        {
            model.BuildingWithTeam = await buildingService.GetAllAsync();
            model.EmployeesInTeam = await employeeService.GetAllAsync();
            model.JobsDoneByTeam = await jobDoneService.GetAllAsync();

            try
            {
                var entity = await teamRepository.GetByIdAsync(model.Id);

                foreach (var jobDoneId in model.JobsDoneByTeamIds)
                {
                    if (!jobDoneTeamMappingService.Any(jobDoneId, model.Id))
                        await jobDoneTeamMappingService.AddAsync(jobDoneId, model.Id);
                }

                foreach (var buildingId in model.BuildingWithTeamIds)
                {
                    if (!buildingTeamMappingService.Any(buildingId, model.Id))
                        await buildingTeamMappingService.AddAsync(buildingId, model.Id);
                }

                foreach (var employeeId in model.EmployeesInTeamIds)
                {
                    if (!employeeTeamMappingService.Any(employeeId, model.Id))
                        await employeeTeamMappingService.AddAsync(employeeId, model.Id);
                }

                var existingEmployeeMappings = await employeeTeamMappingService.GetByTeamIdAsync(model.Id);
                var employeesToRemove = existingEmployeeMappings
                    .Where(mapping => !model.EmployeesInTeamIds.Contains(mapping.EmployeeId))
                    .ToList();

                foreach (var mapping in employeesToRemove)
                {
                    await employeeTeamMappingService.RemoveAsync(mapping);
                }

                var existingBuildingMappings = await buildingTeamMappingService.GetByTeamIdAsync(model.Id);
                var buildingsToRemove = existingBuildingMappings
                    .Where(mapping => !model.BuildingWithTeamIds.Contains(mapping.BuildingId))
                    .ToList();

                foreach (var mapping in buildingsToRemove)
                {
                    await buildingTeamMappingService.RemoveAsync(mapping);
                }

                var existingJobDoneMappings = await jobDoneTeamMappingService.GetByTeamIdAsync(model.Id);
                var jobsToRemove = existingJobDoneMappings
                    .Where(mapping => !model.JobsDoneByTeamIds.Contains(mapping.JobDoneId))
                    .ToList();

                foreach (var mapping in jobsToRemove)
                {
                    await jobDoneTeamMappingService.RemoveAsync(mapping);
                }

                AutoMapperConfig.MapperInstance.Map(model, entity);

                await teamRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all teams that are not marked as deleted.
        /// Maps them to TeamAllViewModel.
        /// </summary>
        /// <returns>IEnumerable of TeamAllViewModel.</returns>
        public async Task<ICollection<TeamViewModel>> GetAllAsync()
                => await teamRepository
                .GetAllAttached()
                .Include(x => x.JobsDoneByTeam)
                .ThenInclude(jd => jd.JobDone)
                .Include(x => x.BuildingWithTeam)
                .ThenInclude(b => b.Building)
                .Include(x => x.EmployeesInTeam)
                .ThenInclude(e => e.Employee)
                .To<TeamViewModel>()
                .ToListAsync();

        public IQueryable<Team> GetAllAttached()
            => teamRepository
            .GetAllAttached();

        /// <summary>
        /// Retrieves a specific team by its ID.
        /// Throws an exception if the team is not found.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamViewModel mapped from the retrieved team.</returns>
        public async Task<TeamViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            TeamViewModel? model = await teamRepository.GetAllAttached()
                                    .Include(t => t.BuildingWithTeam)
                                    .ThenInclude(t => t.Building)
                                    .Include(t => t.JobsDoneByTeam)
                                    .ThenInclude(t => t.JobDone)
                                    .Include(t => t.EmployeesInTeam)
                                    .ThenInclude(t => t.Employee)
                                    .To<TeamViewModel>()
                                    .FirstOrDefaultAsync(t => t.Id == validId);
            return model ?? throw new ArgumentException("Missing entity.");
        }

        /// <summary>
        /// Performs a soft delete of a team, marking it as deleted without removing it from the database.
        /// Returns true if the deletion was successful, otherwise false.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await teamRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Any(Guid id)
        {
            var team = await teamRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (team == null) return false;
            return true;
        }

        /// <summary>
        /// Converts a string ID to a GUID and verifies its validity.
        /// Throws an exception if the ID is null, empty, or invalid.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>Valid Guid ID.</returns>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        private string GetUserId()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("Failed to retrieve UserId. Please try again.");
            return userId;
        }

    }
}
