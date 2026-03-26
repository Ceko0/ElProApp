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
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Application.Services.Interfaces;

    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    /// <summary>
    /// Provides application-level operations for managing job done records.
    /// </summary>
    public class JobDoneService : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly IHelpMethodsService helpMethodsService;
        private readonly IMaterialConsumptionService materialConsumptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneService"/> class.
        /// </summary>
        public JobDoneService(
            IRepository<JobDone, Guid> jobDoneRepository,
            IServiceProvider serviceProvider,
            IHelpMethodsService helpMethodsService,
            IMaterialConsumptionService materialConsumptionService)
        {
            this.jobDoneRepository = jobDoneRepository;
            this.serviceProvider = serviceProvider;
            this.helpMethodsService = helpMethodsService;
            this.materialConsumptionService = materialConsumptionService;
        }

        /// <summary>
        /// Prepares a new <see cref="JobDoneInputModel"/> with required lookup data.
        /// </summary>
        public async Task<JobDoneInputModel> AddAsync()
        {
            var jobService = serviceProvider.GetRequiredService<IJobService>();
            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            string userId = helpMethodsService.GetUserId();

            var model = new JobDoneInputModel
            {
                Teams = await employeeTeamMappingService
                    .GetAllAttached()
                    .Include(x => x.Team)
                    .Include(x => x.Employee)
                    .Where(x => x.Employee.UserId == userId)
                    .Select(x => x.Team)
                    .ToListAsync(),

                JobsList = await jobService
                    .GetAllAttached()
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(),

                Jobs = await jobService
                    .GetAllAttached()
                    .Where(x => !x.IsDeleted)
                    .ToDictionaryAsync(x => x.Id, _ => 0m),

                Buildings = await buildingService
                    .GetAllAttached()
                    .Where(x => !x.IsDeleted)
                    .ToListAsync()
            };

            return model;
        }

        /// <summary>
        /// Creates a new job done record.
        /// </summary>
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var buildingService = 
                serviceProvider.GetRequiredService<IBuildingService>();
            var jobDoneTeamMappingService =
                serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var calculator =
                serviceProvider.GetRequiredService<IEarningsCalculationService>();
            var buildingTeamMappingService =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            var jobDone =
                AutoMapperConfig.MapperInstance.Map<JobDone>(model);

            var team = await helpMethodsService
                .GetAllTeams()
                .FirstOrDefaultAsync(x => x.Id == model.TeamId);

            jobDone.Name = $"От {model.StartDate:dd.MM.yyyy} до {model.EndDate:dd.MM.yyyy} Екип : {team?.Name}";

            jobDone.Building = await buildingService
                .GetAllAttached()
                .FirstOrDefaultAsync(x => x.Id == model.BuildingId);

            await jobDoneRepository.AddAsync(jobDone);

            await jobDoneTeamMappingService.AddAsync(jobDone.Id, model.TeamId);

            if (!buildingTeamMappingService.Any(model.BuildingId, model.TeamId))
            {
                await buildingTeamMappingService
                    .AddAsync(model.BuildingId, model.TeamId);
            }

            await calculator.CalculateMoneyAsync(
                model.TeamId,
                model.Jobs,
                jobDone.Id,
                model.DaysForJob,
                Add);

            await materialConsumptionService.ApplyAsync(
                jobDone.Id,
                model.BuildingId,
                model.Jobs);

            return jobDone.Id.ToString();
        }

        /// <summary>
        /// Retrieves a job done edit model by identifier.
        /// </summary>
        public async Task<JobDoneEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException(
                    "JobDone record not found or is deleted.");

            var team = await helpMethodsService.GetTeamInforamtion(entity.Id);

            var model = new JobDoneEditInputModel
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingId = entity.BuildingId,
                Building = entity.Building,
                DaysForJob = entity.DaysForJob,
                TeamId = team.Id,
                Team = team
            };

            model.JobList = await serviceProvider
                .GetRequiredService<IJobService>()
                .GetAllAttached()
                .ToListAsync();

            model.Jobs = await serviceProvider
                .GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            model.Materials = await materialMappingService
                .GetAllAttached()
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.MaterialId, x => x.Quantity);

            return model;
        }

        /// <summary>
        /// Updates an existing job done record, recalculates earnings,
        /// and synchronizes material consumption.
        /// </summary>
        /// <param name="model">The edit model containing updated data.</param>
        /// <returns>True if the update was successful.</returns>
        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await jobDoneRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("JobDone record not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("JobDone record is deleted.");

            var jobDoneJobService =
                serviceProvider.GetRequiredService<IJobDoneJobMappingService>();

            var calculator =
                serviceProvider.GetRequiredService<IEarningsCalculationService>();

            var buildingTeamMappingService =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            await materialConsumptionService.RollbackAsync(model.Id);

            int oldDays = entity.DaysForJob;

            var previousJobs = await jobDoneJobService
                .GetAllAttached()
                .Where(x => x.JobDoneId == model.Id)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            var oldMappings = await jobDoneJobService
                .GetAllAttached()
                .Where(x => x.JobDoneId == model.Id)
                .ToListAsync();

            foreach (var mapping in oldMappings)
            {
                await jobDoneJobService.RemoveAsync(mapping.JobDoneId, mapping.JobId);
            }

            await calculator.CalculateMoneyAsync(
                model.TeamId,
                previousJobs,
                model.Id,
                oldDays,
                Remove);

            entity.Name = model.Name;
            entity.DaysForJob = model.DaysForJob;
            entity.BuildingId = model.BuildingId;

            await jobDoneRepository.SaveAsync();

            if (!buildingTeamMappingService.Any(model.BuildingId, model.TeamId))
            {
                await buildingTeamMappingService.AddAsync(model.BuildingId, model.TeamId);
            }

            await materialConsumptionService.ApplyAsync(
                model.Id,
                model.BuildingId,
                model.Materials);

            await calculator.CalculateMoneyAsync(
                model.TeamId,
                model.Jobs,
                model.Id,
                model.DaysForJob,
                Add);

            return true;
        }

        /// <summary>
        /// Retrieves all job done records.
        /// </summary>
        public async Task<ICollection<JobDoneViewModel>> GetAllAsync()
        {
            var entities = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            var models =
                AutoMapperConfig.MapperInstance.Map<List<JobDoneViewModel>>(entities);

            foreach (var jobDone in models)
            {
                var team = await helpMethodsService
                    .GetTeamInforamtion(jobDone.Id);

                jobDone.TeamId = team.Id;
                jobDone.Team = team;
            }

            return models;
        }

        /// <summary>
        /// Returns all attached, non-deleted job done records.
        /// </summary>
        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a job done record by identifier.
        /// </summary>
        public async Task<JobDoneViewModel> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new ArgumentException("JobDone record not found.");

            var model =
                AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);

            var jobDoneTeamMappingService =
                serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            var mapping =
                await jobDoneTeamMappingService.GetByJobDoneIdAsync(model.Id);

            model.TeamId = mapping.TeamId;
            model.Team = mapping.Team;

            model.Jobs = await serviceProvider
                .GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Include(x => x.Job)
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            model.JobsList = await serviceProvider
                .GetRequiredService<IJobService>()
                .GetAllAttached()
                .ToListAsync();

            return model;
        }

        /// <summary>
        /// Soft deletes a job done record and recalculates earnings.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(string id, string teamId)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException("JobDone record not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("JobDone already deleted.");

            var jobs = await serviceProvider
                .GetRequiredService<IJobDoneJobMappingService>()
                .GetAllAttached()
                .Where(x => x.JobDoneId == validId)
                .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

            await materialConsumptionService.RollbackAsync(validId);

            var calculator =
                serviceProvider.GetRequiredService<IEarningsCalculationService>();

            await calculator.CalculateMoneyAsync(
                helpMethodsService.ConvertAndTestIdToGuid(teamId),
                jobs,
                entity.Id,
                entity.DaysForJob,
                Remove);

            bool isDeleted = await jobDoneRepository.SoftDeleteAsync(validId);

            return isDeleted;
        }
    }
}
