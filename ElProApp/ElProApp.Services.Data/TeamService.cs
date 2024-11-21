﻿namespace ElProApp.Services.Data
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
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class TeamService(IRepository<Team, Guid> _teamRepository
        , IHttpContextAccessor _httpContextAccessor
        , IServiceProvider _serviceProvider)
        : ITeamService
    {
        private readonly IHttpContextAccessor httpContextAccessor = _httpContextAccessor;
        private readonly IRepository<Team, Guid> teamRepository = _teamRepository;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        public async Task<TeamInputModel> AddAsync()
        {
            var model = new TeamInputModel()
            {
                BuildingWithTeam = await GetAllBuildings().To<BuildingViewModel>().ToListAsync(),

                AvailableEmployees = await GetAllEmployees().To<EmployeeViewModel>().ToListAsync(),

                JobsDoneByTeam = await GetAllJobDones().To<JobDoneViewModel>().ToListAsync()
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

            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();


            if (await teamRepository.FirstOrDefaultAsync(x => x.Name == model.Name) != null)
                throw new InvalidOperationException("A team with this name already exists!");

            var team = AutoMapperConfig.MapperInstance.Map<Team>(model);

            await teamRepository.AddAsync(team);

            if (model.SelectedBuildingId != Guid.Empty)
            {
                BuildingViewModel building = await GetAllBuildings().To<BuildingViewModel>().FirstOrDefaultAsync(x => x.Id == model.SelectedBuildingId)
                    ?? throw new InvalidOperationException("The selected building does not exist.");
                await teamRepository.AddAsync(team);


                var buildingTeamMapping = await buildingTeamMappingService.AddAsync(building.Id, team.Id);

            }

            var userId = ConvertAndTestIdToGuid(GetUserId());

            var employeEntity = await GetAllEmployees().FirstOrDefaultAsync(x => x.Id == userId);
            if (employeEntity != null)
            {
                var employeeTeamMapping = await employeeTeamMappingService.AddAsync(employeEntity.Id, team.Id);
            }

            if (model.SelectedEmployeeIds.Count > 0)
            {
                foreach (var employeeId in model.SelectedEmployeeIds)
                {
                    var employeeMapping = await employeeTeamMappingService.AddAsync(employeeId, team.Id);
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
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            Guid validId = ConvertAndTestIdToGuid(id);
            Team entity = await teamRepository.GetByIdAsync(validId);
            if (entity.IsDeleted) throw new InvalidOperationException("Team is deleted.");
            var userId = ConvertAndTestIdToGuid(GetUserId());
            var employeeTeamMapping = employeeTeamMappingService.GetAllAttached().Where(x => x.TeamId == entity.Id && x.EmployeeId == userId);

            if (employeeTeamMapping == null) throw new AccessViolationException("User does not have permission to edit this team.");

            var model = new TeamEditInputModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingWithTeam = new List<BuildingViewModel>(
                    await GetAllBuildings()
                    .To<BuildingViewModel>()
                    .ToListAsync()),
                JobsDoneByTeam = new List<JobDoneViewModel>(
                    await GetAllJobDones()
                    .To<JobDoneViewModel>()
                    .ToListAsync()),
                EmployeesInTeam = new List<EmployeeViewModel>(
                    await GetAllEmployees()
                    .To<EmployeeViewModel>()
                    .ToListAsync()),
                BuildingWithTeamIds = new List<Guid>(
                    GetAllBuildingTeamMappings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.BuildingId)),
                JobsDoneByTeamIds = new List<Guid>(
                    GetAllJobDoneTeamMappings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.JobDoneId)),
                EmployeesInTeamIds = new List<Guid>(
                    GetAllEmployeeTeamMppings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.EmployeeId))
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
            model.BuildingWithTeam = new List<BuildingViewModel>(
                   await GetAllBuildings()
                   .To<BuildingViewModel>()
                   .ToListAsync());
            model.JobsDoneByTeam = new List<JobDoneViewModel>(
                await GetAllJobDones()
                .To<JobDoneViewModel>()
                .ToListAsync());
            model.EmployeesInTeam = new List<EmployeeViewModel>(
                await GetAllEmployees()
                .To<EmployeeViewModel>()
                .ToListAsync());

            var existingBuildingMappings = await GetAllBuildingTeamMappings().Where(x => x.TeamId == model.Id).ToListAsync();
            var existingEmployeeMappings = await GetAllEmployeeTeamMppings().Where(x => x.TeamId == model.Id).ToListAsync();
            var existingJobDoneMappings = await GetAllJobDoneTeamMappings().Where(x => x.TeamId == model.Id).ToListAsync();

            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            try
            {
                var entity = await teamRepository.GetByIdAsync(model.Id);

                foreach (var jobDoneId in model.JobsDoneByTeamIds)
                {
                    if (!jobDoneTeamMappingService.Any(jobDoneId, model.Id))
                        await jobDoneTeamMappingService.AddAsync(jobDoneId, model.Id);
                }

                var jobsToRemove = existingJobDoneMappings
                    .Where(mapping => !model.JobsDoneByTeamIds.Contains(mapping.JobDoneId))
                    .ToList();

                foreach (var mapping in jobsToRemove)
                {
                    await jobDoneTeamMappingService.RemoveAsync(mapping);
                }

                foreach (var buildingId in model.BuildingWithTeamIds)
                {
                    if (!buildingTeamMappingService.Any(buildingId, model.Id))
                        await buildingTeamMappingService.AddAsync(buildingId, model.Id);
                }

                var buildingsToRemove = existingBuildingMappings
                    .Where(mapping => !model.BuildingWithTeamIds.Contains(mapping.BuildingId))
                    .ToList();

                foreach (var mapping in buildingsToRemove)
                {
                    await buildingTeamMappingService.RemoveAsync(mapping);
                }

                foreach (var employeeId in model.EmployeesInTeamIds)
                {
                    if (!employeeTeamMappingService.Any(employeeId, model.Id))
                        await employeeTeamMappingService.AddAsync(employeeId, model.Id);
                }

                var employeesToRemove = existingEmployeeMappings
                    .Where(mapping => !model.EmployeesInTeamIds.Contains(mapping.EmployeeId))
                    .ToList();

                foreach (var mapping in employeesToRemove)
                {
                    await employeeTeamMappingService.RemoveAsync(mapping);
                }

                Team team = AutoMapperConfig.MapperInstance.Map(model, entity);

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
        {
            var model = await teamRepository
                .GetAllAttached()
                .To<TeamViewModel>()
                .ToListAsync();

            foreach(var viewmodel in model)
            {
                viewmodel.EmployeesInTeam = await GetAllEmployeeTeamMppings().Include(x => x.Employee).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
                viewmodel.JobsDoneByTeam = await GetAllJobDoneTeamMappings().Include(x => x.JobDone).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
                viewmodel.BuildingWithTeam = await GetAllBuildingTeamMappings().Include(x => x.Building).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
            }

            return model;
        }
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
                                    .To<TeamViewModel>()
                                    .FirstOrDefaultAsync(t => t.Id == validId);

            model.EmployeesInTeam = await GetAllEmployeeTeamMppings().Include(x => x.Employee).Where(x => x.TeamId == model.Id).ToListAsync();
            model.JobsDoneByTeam = await GetAllJobDoneTeamMappings().Include(x => x.JobDone).Where(x => x.TeamId == model.Id).ToListAsync();
            model.BuildingWithTeam = await GetAllBuildingTeamMappings().Include(x => x.Building).Where(x => x.TeamId == model.Id).ToListAsync();

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

        private IQueryable<Building> GetAllBuildings()
        {
            var service = serviceProvider.GetRequiredService<IBuildingService>();
            var model = service.GetAllAttached();

            return model;
        }
        private IQueryable<Employee> GetAllEmployees()
        {
            var service = serviceProvider.GetRequiredService<IEmployeeService>();
            var model = service.GetAllAttached();

            return model;
        }
        private IQueryable<JobDone> GetAllJobDones()
        {
            var service = serviceProvider.GetRequiredService<IJobDoneService>();
            var model = service.GetAllAttached();

            return model;
        }
        private IQueryable<Job> GetAllJobs()
        {
            var service = serviceProvider.GetRequiredService<IJobService>();
            var model= service.GetAllAttached();

            return model;
        }
        private IQueryable<BuildingTeamMapping> GetAllBuildingTeamMappings()
        {
            var service = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var model = service.GetAllAttached();

            return model;
        }
        private IQueryable<EmployeeTeamMapping> GetAllEmployeeTeamMppings()
        {
            var service = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var model = service.GetAllAttached();

            return model;
        }
        private IQueryable<JobDoneTeamMapping> GetAllJobDoneTeamMappings()
        {
            var service = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var model = service.GetAllAttached();

            return model;
        }
    }
}
