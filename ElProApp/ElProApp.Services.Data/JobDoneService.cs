namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;
    using Mapping;
    using Web.Models.JobDone;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    /// <summary>
    /// Service class for managing job done operations, including adding, editing, retrieving, and deleting job done records.
    /// </summary>
    public class JobDoneService(IRepository<JobDone, Guid> _jobDoneRepository, IServiceProvider _serviceProvider)
        : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository = _jobDoneRepository;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        /// <summary>
        /// Initializes a new job done input model with lists of teams and jobs.
        /// </summary>
        /// <returns>A <see cref="JobDoneInputModel"/> containing lists of available teams and jobs.</returns>
        public async Task<JobDoneInputModel> AddAsync()
        {
            var TeamService = serviceProvider.GetRequiredService<ITeamService>();
            var jobService = serviceProvider.GetRequiredService<IJobService>();
            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();

            var model = new JobDoneInputModel();
            model.teams = await TeamService.GetAllAttached().Where(x => !x.IsDeleted).ToListAsync();
            model.jobs = await jobService.GetAllAttached().Where(x => !x.IsDeleted).ToListAsync();
            model.buildings = await buildingService.GetAllAttached().Where(x => !x.IsDeleted).ToListAsync();
            return model;
        }

        /// <summary>
        /// Adds a new job done record.
        /// </summary>
        /// <param name="model">The input model containing data for the job done.</param>
        /// <returns>The ID of the newly created job done record as a string.</returns>
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            var jobDoneTeamMppingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            var jobService = serviceProvider.GetRequiredService<IJobService>();

            var currentJob = await jobService.GetAllAttached().FirstOrDefaultAsync(x => x.Id == model.JobId && !x.IsDeleted);
            if (currentJob == null)
                throw new InvalidOperationException("Job not found.");

            var teamService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            var team = await teamService
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.Building)
                .Where(x => x.TeamId == model.TeamId
                                && x.BuildingId == model.BuildingId
                                && !x.Team.IsDeleted
                                && !x.Building.IsDeleted)
                .Select(x => x.Team)
                .ToListAsync();

            if (team == null)
                team = new List<Team>();

            var jobDone = AutoMapperConfig.MapperInstance.Map<JobDone>(model);

            currentJob?.JobsDone.Append(jobDone);

            await jobDoneRepository.AddAsync(jobDone);
            await jobDoneTeamMppingService.AddAsync(model.Id, model.TeamId);
            var calculator = serviceProvider.GetRequiredService<IEarningsCalculationService>();
            await calculator.CalculateMoneyAsync(model);
            return jobDone.Id.ToString();
        }

        /// <summary>
        /// Retrieves the job done edit input model for a specific job done record by its ID.
        /// </summary>
        /// <param name="id">The ID of the job done record to retrieve.</param>
        /// <returns>A <see cref="JobDoneEditInputModel"/> for editing the job done record.</returns>
        public async Task<JobDoneEditInputModel> EditByIdAsync(string id )
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            
            var model = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Job)
                .Include( x => x.Building)
                .Where(x => !x.IsDeleted)
                .To<JobDoneEditInputModel>()
                .FirstOrDefaultAsync(x => x.Id == validId);
            if (model == null)
                throw new InvalidOperationException("Jobdone is deleted or not found.");

            model.Team = await GetTeamInforamtion(model.Id);
            model.TeamId = model.Team.Id;
            return model!;
        }

        /// <summary>
        /// Updates the job done record with the data from the provided edit model.
        /// </summary>
        /// <param name="model">The edit input model containing updated job done data.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            try
            {
                var entity = await jobDoneRepository.GetByIdAsync(model.Id);
                if (entity == null || entity.IsDeleted)
                    throw new InvalidOperationException("Jobdone is deleted or not found.");

                AutoMapperConfig.MapperInstance.Map(model, entity);

                await jobDoneRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all job done records.
        /// </summary>
        /// <returns>A collection of <see cref="JobDoneViewModel"/> representing all job done records.</returns>
        public async Task<ICollection<JobDoneViewModel>> GetAllAsync()
        {
            var model = await jobDoneRepository.GetAllAttached()
                .Include(x => x.Job)
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .To<JobDoneViewModel>()
                .ToListAsync();

            foreach (var jobDone in model)
            {
                var team = await GetTeamInforamtion(jobDone.Id);
                jobDone.TeamId = team.Id;
                jobDone.Team = team;
            }
            return model;
        }

        /// <summary>
        /// Retrieves all job done records, including the associated job and team information.
        /// </summary>
        /// <returns>An <see cref="IQueryable"/> collection of <see cref="JobDone"/> records with attached data.</returns>
        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository.GetAllAttached().Include(x => x.Job).Where(x => !x.IsDeleted && !x.Job.IsDeleted);

        /// <summary>
        /// Retrieves a specific job done record by its ID.
        /// </summary>
        /// <param name="id">The ID of the job done record to retrieve.</param>
        /// <returns>A <see cref="JobDoneViewModel"/> representing the job done record, or throws an exception if not found.</returns>
        public async Task<JobDoneViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Include(x => x.Job)
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (entity != null)
            {
                JobDoneViewModel? model = AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);
                var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

                var maping = await jobDoneTeamMappingService.GetByJobDoneIdAsync(model.Id);
                model.TeamId = maping.TeamId;
                model.Team = maping.Team;
                return model;
            }

            throw new ArgumentException("Missing entity.");
        }

        /// <summary>
        /// Soft deletes a job done record by marking it as deleted.
        /// </summary>
        /// <param name="id">The ID of the job done record to delete.</param>
        /// <returns>True if the delete operation was successful, otherwise false.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                var entity = await jobDoneRepository.GetByIdAsync(validId);
                if (entity == null)
                    throw new InvalidOperationException("JobDone record not found.");

                bool isDeleted = await jobDoneRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts and validates a string ID to a valid <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The string ID to convert and validate.</param>
        /// <returns>A valid <see cref="Guid"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the ID format is invalid.</exception>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        private async Task<Team> GetTeamInforamtion(Guid id)
        {
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var allTeamMappings = await jobDoneTeamMappingService
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.JobDone)
                .ToListAsync();

            var currentTeam = allTeamMappings.FirstOrDefault(x => x.JobDoneId == id);
            return currentTeam!.Team;

        }
    }
}
