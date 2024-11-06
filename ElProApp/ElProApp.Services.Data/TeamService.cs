namespace ElProApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Team;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Data.Models.Mappings;

    public class TeamService(IRepository<Team, Guid> _teamRepository
        , IBuildingService _buildingService
        , IEmployeeTeamMappingService _employeeTeamMpaping
        , IBuildingTeamMappingService _buildingTeamMappingService
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
        private readonly IEmployeeService employeeService = _employeeService;
        private readonly IJobDoneService jobDoneService = _jobDoneService;

        public async Task<TeamInputModel> AddAsync()
        {
            // Създаваме модел с избрани членове, празен списък за нов екип
            var model = new TeamInputModel()
            {
                // Зареждаме списъка със сгради за dropdown менюто
                BuildingWithTeam = (ICollection<BuildingViewModel>)await buildingService.GetAllAsync(),

                // Зареждаме всички налични служители, за да се покажат като чекбоксове
                AvailableEmployees = (ICollection<EmployeeViewModel>)await employeeService.GetAllAsync(),

                // Зареждаме всички налични завършени задачи, за да се покажат
                JobsDoneByTeam = (ICollection<JobDoneViewModel>)await jobDoneService.GetAllAsync(),
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
                var building = await buildingService.GetByIdAsync(model.SelectedBuildingId.ToString());

                if (building == null)
                    throw new InvalidOperationException("The selected building does not exist.");

                await teamRepository.AddAsync(team);
                string stringUserId = GetUserId();
                if (!Guid.TryParse(stringUserId, out Guid userId)) throw new ArgumentException("Invalid user id");

                var employeEntity = employeeService.GetByUserId(stringUserId);

                var employeeTeamMapping = await employeeTeamMappingService.AddAsync(employeEntity.Id, team.Id);

                var buildingTeamMapping = await buildingTeamMappingService.AddAsync(building.Id, team.Id);

                building.TeamsOnBuilding.Add(buildingTeamMapping);
                team.BuildingWithTeam.Add(buildingTeamMapping);

                employeEntity.TeamsEmployeeBelongsTo.Add(employeeTeamMapping);
                team.EmployeesInTeam.Add(employeeTeamMapping);

                foreach (var employeeId in model.SelectedEmployeeIds)
                {
                    var employeeMapping = await employeeTeamMappingService.AddAsync(employeeId, team.Id);
                    var employee = await employeeService.GetByIdAsync(employeeId.ToString());
                    employee?.TeamsEmployeeBelongsTo.Add(employeeMapping);
                    team.EmployeesInTeam.Add(employeeTeamMapping);
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
            var validUserId = ConvertAndTestIdToGuid(userId);
            if (!entity.EmployeesInTeam.Any(x => x.EmployeeId == validUserId)) throw new AccessViolationException("User does not have permission to edit this team.");

            return AutoMapperConfig.MapperInstance.Map<TeamEditInputModel>(entity);
        }

        /// <summary>
        /// Updates a team entity based on the provided input model.
        /// Returns true if the update was successful, otherwise false.
        /// </summary>
        /// <param name="model">TeamEditInputModel containing updated team data.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> EditByModelAsync(TeamEditInputModel model)
        {
            try
            {
                var entity = await teamRepository.GetByIdAsync(model.Id);
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
        public async Task<IEnumerable<TeamViewModel>> GetAllAsync()
        {
            var teams = await teamRepository.GetAllAttached()
                                   .Include(t => t.EmployeesInTeam)
                                   .ThenInclude(et => et.Employee)
                                   .Include(t => t.JobsDoneByTeam)
                                   .ThenInclude(jdt => jdt.JobDone)
                                   .Include(t => t.BuildingWithTeam)
                                   .ThenInclude(bt => bt.Building)
                                   .Where(x => !x.IsDeleted)
                                   .To<TeamViewModel>()
                                   .ToListAsync();

            return (teams);
        }
        /// <summary>
        /// Retrieves a specific team by its ID.
        /// Throws an exception if the team is not found.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamViewModel mapped from the retrieved team.</returns>
        public async Task<TeamViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await teamRepository.GetAllAttached()
                                    .Include(t => t.BuildingWithTeam)
                                    .ThenInclude(bt => bt.Building)
                                    .Include(t => t.JobsDoneByTeam)
                                    .ThenInclude(jt => jt.JobDone)
                                    .Include(t => t.EmployeesInTeam)
                                    .ThenInclude(et => et.Employee)
                                    .FirstOrDefaultAsync(t => t.Id == validId)
                                    .ConfigureAwait(false);

            return entity != null
                ? AutoMapperConfig.MapperInstance.Map<TeamViewModel>(entity)
                : throw new ArgumentException("Missing entity.");
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
