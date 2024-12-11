namespace ElProApp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;
    using Mapping;
    using Web.Models.Building;
    using Web.Models.Employee;
    using Web.Models.JobDone;
    using Web.Models.Team;

    public class TeamService(IRepository<Team, Guid> teamRepository, 
                             IServiceProvider serviceProvider, 
                             IHelpMethodsService helpMethodsService) : 
                             ITeamService
    {
        /// <summary>
        /// Creates a new TeamInputModel with a list of available buildings, employees, and jobs.
        /// </summary>
        /// <returns>A TeamInputModel with all the necessary data for adding a new team.</returns>
        public async Task<TeamInputModel> AddAsync()
        {
            var model = new TeamInputModel()
            {
                BuildingWithTeam = await helpMethodsService.GetAllBuildings().Where(x => !x.IsDeleted).To<BuildingViewModel>().ToListAsync(),

                AvailableEmployees = await helpMethodsService.GetAllEmployees().Where(x => !x.IsDeleted).To<EmployeeViewModel>().ToListAsync(),

                JobsDoneByTeam = await helpMethodsService.GetAllJobDones().Where(x => !x.IsDeleted).To<JobDoneViewModel>().ToListAsync()
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

            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            var team = AutoMapperConfig.MapperInstance.Map<Team>(model);

            await teamRepository.AddAsync(team);

            if (model.SelectedBuildingId != Guid.Empty)
            {
                var building = await helpMethodsService.GetAllBuildings().Where(x => !x.IsDeleted).To<BuildingViewModel>().FirstOrDefaultAsync(x => x.Id == model.SelectedBuildingId)
                    ?? throw new InvalidOperationException("The selected building does not exist.");
                await buildingTeamMappingService.AddAsync(building.Id, team.Id);
            }

            var userId = helpMethodsService.ConvertAndTestIdToGuid(helpMethodsService.GetUserId());

            var employeeEntity = await helpMethodsService.GetAllEmployees().Where(x => !x.IsDeleted).FirstOrDefaultAsync(x => x.Id == userId);
            if (employeeEntity != null)
            {
                await employeeTeamMappingService.AddAsync(employeeEntity.Id, team.Id);
            }

            if (model.SelectedEmployeeIds.Count > 0)
            {
                foreach (var employeeId in model.SelectedEmployeeIds)
                {
                    await employeeTeamMappingService.AddAsync(employeeId, team.Id);
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
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            Team entity = await teamRepository.GetByIdAsync(validId);
            if (entity == null || entity.IsDeleted)
                throw new InvalidOperationException("Team is deleted or not found.");

            var userId = helpMethodsService.ConvertAndTestIdToGuid(helpMethodsService.GetUserId());

            var employeeTeamMapping = await helpMethodsService.GetAllEmployeeTeamMаppings()
                .Where(x => x.TeamId == entity.Id && x.Employee.UserId == userId.ToString())
            .FirstOrDefaultAsync();
            var user = await helpMethodsService.GetUserAsync(userId.ToString());
            var roles = await helpMethodsService.GetUserRolesAsync(user);
            if (!roles.Contains("Admin") && employeeTeamMapping == null) throw new UnauthorizedAccessException();

            var model = new TeamEditInputModel
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingWithTeam = await helpMethodsService.GetAllBuildings().To<BuildingViewModel>().ToListAsync(),
                JobsDoneByTeam = await helpMethodsService.GetAllJobDones().To<JobDoneViewModel>().ToListAsync(),
                EmployeesInTeam = await helpMethodsService.GetAllEmployees().To<EmployeeViewModel>().ToListAsync(),
                BuildingWithTeamIds = await helpMethodsService.GetAllBuildingTeamMappings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.BuildingId)
                    .ToListAsync(),
                JobsDoneByTeamIds = await helpMethodsService.GetAllJobDoneTeamMappings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.JobDoneId)
                    .ToListAsync(),
                EmployeesInTeamIds = await helpMethodsService.GetAllEmployeeTeamMаppings()
                    .Where(x => x.TeamId == entity.Id)
                    .Select(x => x.EmployeeId)
                    .ToListAsync()
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
            var userId = helpMethodsService.ConvertAndTestIdToGuid(helpMethodsService.GetUserId());
            var employeeTeamMapping = await helpMethodsService.GetAllEmployeeTeamMаppings()
                .Where(x => x.TeamId == model.Id && x.Employee.UserId == userId.ToString())
                .FirstOrDefaultAsync();
            var user = await helpMethodsService.GetUserAsync(userId.ToString());
            var roles = await helpMethodsService.GetUserRolesAsync(user);
            if (!roles.Contains("Admin") && employeeTeamMapping == null) throw new UnauthorizedAccessException();

            model.BuildingWithTeam = await helpMethodsService.GetAllBuildings().Where(x => !x.IsDeleted).To<BuildingViewModel>().ToListAsync();
            model.JobsDoneByTeam = await helpMethodsService.GetAllJobDones().Where(x => !x.IsDeleted).To<JobDoneViewModel>().ToListAsync();
            model.EmployeesInTeam = await helpMethodsService.GetAllEmployees().Where(x => !x.IsDeleted).To<EmployeeViewModel>().ToListAsync();

            var existingBuildingMappings = await helpMethodsService.GetAllBuildingTeamMappings().Where(x => x.TeamId == model.Id).ToListAsync();
            var existingEmployeeMappings = await helpMethodsService.GetAllEmployeeTeamMаppings().Where(x => x.TeamId == model.Id).ToListAsync();
            var existingJobDoneMappings = await helpMethodsService.GetAllJobDoneTeamMappings().Where(x => x.TeamId == model.Id).ToListAsync();

            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

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
                .Where(x => !x.IsDeleted)
                .To<TeamViewModel>()
                .ToListAsync();

            foreach (var viewmodel in model)
            {
                viewmodel.EmployeesInTeam = await helpMethodsService.GetAllEmployeeTeamMаppings().Include(x => x.Employee).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
                viewmodel.JobsDoneByTeam = await helpMethodsService.GetAllJobDoneTeamMappings().Include(x => x.JobDone).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
                viewmodel.BuildingWithTeam = await helpMethodsService.GetAllBuildingTeamMappings().Include(x => x.Building).Where(x => x.TeamId == viewmodel.Id).ToListAsync();
            }

            return model;
        }

        /// <summary>
        /// Retrieves all teams that are attached to the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{Team}"/> representing the collection of all teams.
        /// </returns>
        public IQueryable<Team> GetAllAttached()
            => teamRepository
            .GetAllAttached()
            .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a specific team by its ID.
        /// Throws an exception if the team is not found.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamViewModel mapped from the retrieved team.</returns>
        public async Task<TeamViewModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("The ID cannot be null or empty.");

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var currentUserId = helpMethodsService.GetUserId();
            var userFromTeam = await helpMethodsService.GetAllEmployeeTeamMаppings()
                .FirstOrDefaultAsync(x => x.Employee.UserId.ToString() == currentUserId && x.TeamId == validId);

            if (userFromTeam == null) throw new UnauthorizedAccessException("You are not a part of that team");

            TeamViewModel? model = await teamRepository.GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<TeamViewModel>()
                .FirstOrDefaultAsync(t => t.Id == validId);

            if (model == null)
                throw new ArgumentException("Team not found.");

            model.EmployeesInTeam = await helpMethodsService.GetAllEmployeeTeamMаppings().Include(x => x.Employee).Where(x => x.TeamId == model.Id).ToListAsync();
            model.JobsDoneByTeam = await helpMethodsService.GetAllJobDoneTeamMappings().Include(x => x.JobDone).Where(x => x.TeamId == model.Id).ToListAsync();
            model.BuildingWithTeam = await helpMethodsService.GetAllBuildingTeamMappings().Include(x => x.Building).Where(x => x.TeamId == model.Id).ToListAsync();

            return model;
        }


        /// <summary>
        /// Performs a soft delete of a team, marking it as deleted without removing it from the database.
        /// Returns true if the deletion was successful, otherwise false.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            try
            {
                bool isDeleted = await teamRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error occurred while soft deleting team with id {validId}.", ex);
            }
        }


        /// <summary>
        /// Checks if a team with the specified ID exists in the repository.
        /// </summary>
        /// <param name="id">The ID of the team to check for existence.</param>
        /// <returns>
        /// A boolean value indicating whether a team with the specified ID exists.
        /// <c>true</c> if the team exists, <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> Any(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The ID cannot be empty.");

            var team = await teamRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (team == null || team.IsDeleted)
                throw new InvalidOperationException("Team is deleted or not found.");
            return team != null;
        }
    }
}
