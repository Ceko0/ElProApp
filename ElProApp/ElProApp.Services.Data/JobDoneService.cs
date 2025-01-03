namespace ElProApp.Services.Data
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;
    using Mapping;
    using Web.Models.JobDone;
    using static Common.EntityValidationConstants.CalculationAction;

    /// <summary>
    /// Service class for managing job done operations, including adding, editing, retrieving, and deleting job done records.
    /// </summary>
    public class JobDoneService(IRepository<JobDone, Guid> jobDoneRepository,
                                IServiceProvider serviceProvider,
                                IHelpMethodsService helpMethodsService) :
                                IJobDoneService
    {
        /// <summary>
        /// Initializes a new job done input model with lists of Teams and JobsList.
        /// </summary>
        /// <returns>A <see cref="JobDoneInputModel"/> containing lists of available Teams and JobsList.</returns>
        public async Task<JobDoneInputModel> AddAsync()
        {
            var jobService = serviceProvider.GetRequiredService<IJobService>();
            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();
            var userId = helpMethodsService.GetUserId();


            var model = new JobDoneInputModel();
            model.Teams = await serviceProvider.GetRequiredService<IEmployeeTeamMappingService>()
                .GetAllAttached()
                .Include(x => x.Team)
                .Include(x => x.Employee)
                .Where(x => x.Employee.UserId == userId)
                .Select(x => x.Team)
                .ToListAsync();
            model.JobsList = await jobService.GetAllAttached().Where(x => !x.IsDeleted).ToListAsync();
            model.Jobs = await jobService.GetAllAttached().Where(x => !x.IsDeleted)
                .ToDictionaryAsync(x => x.Id, x => 0m);
            model.Buildings = await buildingService.GetAllAttached().Where(x => !x.IsDeleted).ToListAsync();
            return model;
        }

        /// <summary>
        /// Adds a new job done record.
        /// </summary>
        /// <param name="model">The input model containing data for the job done.</param>
        /// <returns>The ID of the newly created job done record as a string.</returns>
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var calculator = serviceProvider.GetRequiredService<IEarningsCalculationService>();
            var jobDone = AutoMapperConfig.MapperInstance.Map<JobDone>(model);

            jobDone.Building = await buildingService.GetAllAttached().FirstOrDefaultAsync(x => x.Id == model.BuildingId);

            await jobDoneRepository.AddAsync(jobDone);
            await jobDoneTeamMappingService.AddAsync(model.Id, model.TeamId);
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();
            if (buildingTeamMappingService.Any(model.BuildingId, model.TeamId) == false ) 
                buildingTeamMappingService.AddAsync(model.BuildingId,model.TeamId);

            await calculator.CalculateMoneyAsync(model.TeamId,model.Jobs,model.Id,model.DaysForJob,Add);

            return jobDone.Id.ToString();
        }

        /// <summary>
        /// Retrieves the job done edit input model for a specific job done record by its ID.
        /// </summary>
        /// <param name="id">The ID of the job done record to retrieve.</param>
        /// <returns>A <see cref="JobDoneEditInputModel"/> for editing the job done record.</returns>
        public async Task<JobDoneEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == validId);

            if (entity == null)
                throw new InvalidOperationException("JobDone is deleted or not found.");

            var team = await helpMethodsService.GetTeamInforamtion(entity.Id);

            var model = new JobDoneEditInputModel
            {
                BuildingId = entity.BuildingId,
                Building = entity.Building,
                DaysForJob = entity.DaysForJob,
                Id = entity.Id,
                Name = entity.Name,
                TeamId = team.Id,
                Team = team
            };

            model.JobList = await serviceProvider.GetRequiredService<IJobService>().GetAllAttached().ToListAsync();

            model.Jobs = await serviceProvider.GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            return model;
        }

        /// <summary>
        /// Updates the job done record with the data from the provided edit model.
        /// </summary>
        /// <param name="model">The edit input model containing updated job done data.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            var entity = await jobDoneRepository.GetByIdAsync(model.Id);
            if (entity == null || entity.IsDeleted)
                throw new InvalidOperationException("JobDone is deleted or not found.");
            var jobsToRemove = await serviceProvider.GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Where(x => x.JobDoneId == model.Id)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);
            var editedJobs = model.Jobs;
            AutoMapperConfig.MapperInstance.Map(model, entity);

            await jobDoneRepository.SaveAsync();

            var calculator = serviceProvider.GetRequiredService<IEarningsCalculationService>();
            await calculator.CalculateMoneyAsync(model.TeamId, jobsToRemove, model.Id, model.DaysForJob, Remove);
            await calculator.CalculateMoneyAsync(model.TeamId, editedJobs, model.Id, model.DaysForJob, Add);
            
            return true;
        }

        /// <summary>
        /// Retrieves all job done records.
        /// </summary>
        /// <returns>A collection of <see cref="JobDoneViewModel"/> representing all job done records.</returns>
        public async Task<ICollection<JobDoneViewModel>> GetAllAsync()
        {
            var model = jobDoneRepository.GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted);

            List<JobDoneViewModel> newModel = new();
            AutoMapperConfig.MapperInstance.Map(model, newModel);

            foreach (var jobDone in newModel)
            {
                var team = await helpMethodsService.GetTeamInforamtion(jobDone.Id);
                jobDone.TeamId = team.Id;
                jobDone.Team = team;
            }
            return newModel;
        }

        /// <summary>
        /// Retrieves all job done records, including the associated job and team information.
        /// </summary>
        /// <returns>An <see cref="IQueryable"/> collection of <see cref="JobDone"/> records with attached data.</returns>
        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository.GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a specific job done record by its ID.
        /// </summary>
        /// <param name="id">The ID of the job done record to retrieve.</param>
        /// <returns>A <see cref="JobDoneViewModel"/> representing the job done record, or throws an exception if not found.</returns>
        public async Task<JobDoneViewModel> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var queryableEntity = jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted);

            var entity = await queryableEntity.FirstOrDefaultAsync(x => x.Id == validId);

            if (entity != null)
            {
                JobDoneViewModel? model = AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);
                var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

                var maping = await jobDoneTeamMappingService.GetByJobDoneIdAsync(model.Id);
                model.TeamId = maping.TeamId;
                model.Team = maping.Team;

                model.Jobs = await serviceProvider.GetRequiredService<IJobDoneJobMappingService>()
                    .GetAllAttached()
                    .Include(x => x.Job)
                    .Include( x => x.JobDone)
                    .Where(x => x.JobDoneId == validId)
                    .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

                model.JobsList = await serviceProvider.GetRequiredService<IJobService>().GetAllAttached().ToListAsync();
                return model;
            }

            throw new ArgumentException("Missing entity.");
        }

        /// <summary>
        /// Soft deletes a job done record by marking it as deleted.
        /// </summary>
        /// <param name="id">The ID of the job done record to delete.</param>
        /// <returns>True if the delete operation was successful, otherwise false.</returns>
        public async Task<bool> SoftDeleteAsync(string id, string teamId)
        {

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var entity = await jobDoneRepository.GetByIdAsync(validId);
            if (entity == null)
                throw new InvalidOperationException("JobDone record not found.");

            bool isDeleted = await jobDoneRepository.SoftDeleteAsync(validId);

            Guid validTeamId = helpMethodsService.ConvertAndTestIdToGuid(teamId);
            var team = await helpMethodsService.GetAllTeam().FirstOrDefaultAsync(x => x.Id == validTeamId) ?? new Team();
            var jobs = await serviceProvider.GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            var calculator = serviceProvider.GetRequiredService<IEarningsCalculationService>();
            await calculator.CalculateMoneyAsync(team.Id , jobs , entity.Id , entity.DaysForJob , Remove);

            return isDeleted;
        }
    }
}
