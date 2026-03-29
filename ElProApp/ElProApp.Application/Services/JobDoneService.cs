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

    /// <summary>
    /// Provides application-level operations for managing job done records.
    /// </summary>
    public class JobDoneService : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly IHelpMethodsService helpMethodsService;
        private readonly IMaterialConsumptionService materialConsumptionService;
        private readonly IBuildingMaterialPriceService priceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneService"/> class.
        /// </summary>
        public JobDoneService(
            IRepository<JobDone, Guid> jobDoneRepository,
            IServiceProvider serviceProvider,
            IHelpMethodsService helpMethodsService,
            IMaterialConsumptionService materialConsumptionService,
            IBuildingMaterialPriceService priceService)
        {
            this.jobDoneRepository = jobDoneRepository;
            this.serviceProvider = serviceProvider;
            this.helpMethodsService = helpMethodsService;
            this.materialConsumptionService = materialConsumptionService;
            this.priceService = priceService;
        }

        /// <summary>
        /// Prepares a new input model.
        /// </summary>
        public async Task<JobDoneInputModel> AddAsync()
        {
            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var materialService = serviceProvider.GetRequiredService<IMaterialService>();

            string userId = helpMethodsService.GetUserId();

            return new JobDoneInputModel
            {
                Teams = await employeeTeamMappingService
                    .GetAllAttached()
                    .Include(x => x.Team)
                    .Include(x => x.Employee)
                    .Where(x => x.Employee.UserId == userId)
                    .Select(x => x.Team)
                    .ToListAsync(),

                Buildings = await buildingService
                    .GetAllAttached()
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(),

                MaterialsList = await materialService
                    .GetAllAttached()
                    .ToListAsync(),

                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
        }

        /// <summary>
        /// Creates a new job done record.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <returns>The created job done identifier.</returns>
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var buildingService = serviceProvider.GetRequiredService<IBuildingService>();
            var jobDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();
            var buildingTeamMappingService = serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            var jobDone = AutoMapperConfig.MapperInstance.Map<JobDone>(model);

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
                await buildingTeamMappingService.AddAsync(model.BuildingId, model.TeamId);
            }

            var materialsDict = model.Materials
                .Where(x => x.Quantity > 0)
                .ToDictionary(x => x.MaterialId, x => x.Quantity);

            var materialsWithPrices = new Dictionary<Guid, (decimal Quantity, decimal Price)>();

            foreach (var kvp in materialsDict)
            {
                var price = await priceService.GetPriceAsync(model.BuildingId, kvp.Key) ?? 0m;
                materialsWithPrices[kvp.Key] = (kvp.Value, price);
            }

            await materialConsumptionService.ApplyAsync(
                jobDone.Id,
                model.BuildingId,
                materialsWithPrices);

            return jobDone.Id.ToString();
        }

        /// <summary>
        /// Retrieves edit model.
        /// </summary>
        /// <param name="id">The job done identifier.</param>
        /// <returns>The edit input model.</returns>
        public async Task<JobDoneEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException("JobDone record not found.");

            var team = await helpMethodsService.GetTeamInforamtion(entity.Id);

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            var materialService =
                serviceProvider.GetRequiredService<IMaterialService>();

            var materials = await materialMappingService
                .GetAllAttached()
                .Include(x => x.Material)
                .Where(x => x.JobDoneId == validId)
                .ToListAsync();

            var model = new JobDoneEditInputModel
            {
                Id = entity.Id,
                Name = entity.Name,
                BuildingId = entity.BuildingId,
                Building = entity.Building,
                DaysForJob = entity.DaysForJob,
                TeamId = team.Id,
                Team = team,

                Materials = materials
                    .Select(x => new MaterialInputPair
                    {
                        MaterialId = x.MaterialId,
                        Quantity = x.Quantity,
                        MaterialName = x.Material.Name
                    })
                    .ToList()
            };

            model.MaterialsList = await materialService
                .GetAllAttached()
                .ToListAsync();

            return model;
        }

        /// <summary>
        /// Updates job done record.
        /// </summary>
        /// <param name="model">The edit model.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await jobDoneRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException("JobDone record not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("JobDone record is deleted.");

            var buildingTeamMappingService =
                serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

            await materialConsumptionService.RollbackAsync(model.Id);

            entity.Name = model.Name;
            entity.DaysForJob = model.DaysForJob;
            entity.BuildingId = model.BuildingId;

            await jobDoneRepository.SaveAsync();

            if (!buildingTeamMappingService.Any(model.BuildingId, model.TeamId))
            {
                await buildingTeamMappingService.AddAsync(model.BuildingId, model.TeamId);
            }

            var materialsDict = model.Materials
                .Where(x => x.Quantity > 0)
                .ToDictionary(x => x.MaterialId, x => x.Quantity);

            var materialsWithPrices = new Dictionary<Guid, (decimal Quantity, decimal Price)>();

            foreach (var kvp in materialsDict)
            {
                var price = await priceService.GetPriceAsync(model.BuildingId, kvp.Key) ?? 0m;
                materialsWithPrices[kvp.Key] = (kvp.Value, price);
            }

            await materialConsumptionService.ApplyAsync(
                model.Id,
                model.BuildingId,
                materialsWithPrices);

            return true;
        }

        /// <summary>
        /// Retrieves all job done records.
        /// </summary>
        /// <returns>A collection of job done view models.</returns>
        public async Task<ICollection<JobDoneViewModel>> GetAllAsync()
        {
            var entities = await jobDoneRepository
                .GetAllAttached()
                .Include(x => x.Building)
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            var models = new List<JobDoneViewModel>();

            foreach (var entity in entities)
            {
                var team = await helpMethodsService.GetTeamInforamtion(entity.Id);

                var materials = await materialMappingService
                    .GetAllAttached()
                    .Include(x => x.Material)
                    .Where(x => x.JobDoneId == entity.Id)
                    .ToListAsync();

                var model =
                    AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);

                model.TeamId = team.Id;
                model.Team = team;

                model.Materials = materials
                    .Select(x => new MaterialInputPair
                    {
                        MaterialId = x.MaterialId,
                        Quantity = x.Quantity,
                        MaterialName = x.Material.Name
                    })
                    .ToList();

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Returns all attached records.
        /// </summary>
        /// <returns>An IQueryable of job done records.</returns>
        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);

        /// <summary>
        /// Retrieves a job done record by identifier.
        /// </summary>
        /// <param name="id">The job done identifier.</param>
        /// <returns>The job done view model.</returns>
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

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            var mapping =
                await jobDoneTeamMappingService.GetByJobDoneIdAsync(model.Id);

            model.TeamId = mapping.TeamId;
            model.Team = mapping.Team;

            var materials = await materialMappingService
                .GetAllAttached()
                .Include(x => x.Material)
                .Where(x => x.JobDoneId == validId)
                .ToListAsync();

            model.Materials = materials
                .Select(x => new MaterialInputPair
                {
                    MaterialId = x.MaterialId,
                    Quantity = x.Quantity,
                    MaterialName = x.Material.Name
                })
                .ToList();

            return model;
        }

        /// <summary>
        /// Soft deletes a job done record.
        /// </summary>
        /// <param name="id">The job done identifier.</param>
        /// <param name="teamId">The team identifier.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> SoftDeleteAsync(string id, string teamId)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await jobDoneRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(x => x.Id == validId)
                ?? throw new InvalidOperationException("JobDone record not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("JobDone already deleted.");

            await materialConsumptionService.RollbackAsync(validId);

            return await jobDoneRepository.SoftDeleteAsync(validId);
        }
    }
}