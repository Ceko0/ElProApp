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
    /// Provides application-level operations for managing job done records,
    /// including creation, editing, deletion and retrieval.
    /// Handles material consumption, stock updates and earnings calculations.
    /// </summary>
    public class JobDoneService : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository;
        private readonly IServiceProvider serviceProvider;
        private readonly IHelpMethodsService helpMethodsService;
        private readonly IMaterialConsumptionService materialConsumptionService;
        private readonly IBuildingMaterialPriceService priceService;
        private readonly IEarningsCalculationService earningsCalculationService;
        private readonly ITransactionService transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobDoneService"/> class.
        /// </summary>
        public JobDoneService(
            IRepository<JobDone, Guid> jobDoneRepository,
            IServiceProvider serviceProvider,
            IHelpMethodsService helpMethodsService,
            IMaterialConsumptionService materialConsumptionService,
            IBuildingMaterialPriceService priceService,
            IEarningsCalculationService earningsCalculationService,
            ITransactionService transactionService)
        {
            this.jobDoneRepository = jobDoneRepository;
            this.serviceProvider = serviceProvider;
            this.helpMethodsService = helpMethodsService;
            this.materialConsumptionService = materialConsumptionService;
            this.priceService = priceService;
            this.earningsCalculationService = earningsCalculationService;
            this.transactionService = transactionService;
        }

        /// <summary>
        /// Prepares a model for creating a new job done.
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
        /// Creates a new job done record using transaction.
        /// </summary>
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return await transactionService.ExecuteAsync(async () =>
            {
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

                await materialConsumptionService.ApplyAsync(jobDone.Id, model.BuildingId, materialsWithPrices);

                await earningsCalculationService.CalculateMoneyAsync(
                    model.TeamId,
                    jobDone.Id,
                    jobDone.DaysForJob,
                    materialsWithPrices,
                    Adding);

                return jobDone.Id.ToString();
            });
        }

        /// <summary>
        /// Retrieves edit model for job done.
        /// </summary>
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
        /// Updates an existing job done record using delta logic with transaction
        /// and safe EF tracking handling.
        /// </summary>
        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return await transactionService.ExecuteAsync(async () =>
            {
                var entity = await jobDoneRepository.GetByIdAsync(model.Id)
                    ?? throw new InvalidOperationException("JobDone record not found.");

                if (entity.IsDeleted)
                    throw new InvalidOperationException("JobDone record is deleted.");

                var buildingTeamMappingService =
                    serviceProvider.GetRequiredService<IBuildingTeamMappingService>();

                var materialMappingService =
                    serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

                var buildingMaterialService =
                    serviceProvider.GetRequiredService<IBuildingMaterialMappingService>();

                var earningsService =
                    serviceProvider.GetRequiredService<IEarningsCalculationService>();

                var oldMappings = await materialMappingService
                    .GetAllAttached()
                    .AsNoTracking()
                    .Where(x => x.JobDoneId == model.Id)
                    .ToListAsync();

                var oldMaterialsWithPrices = oldMappings
                    .ToDictionary(x => x.MaterialId, x => (x.Quantity, x.UnitPrice));

                await earningsService.CalculateMoneyAsync(
                    model.TeamId,
                    model.Id,
                    entity.DaysForJob,
                    oldMaterialsWithPrices,
                    Remove);

                entity.Name = model.Name;
                entity.DaysForJob = model.DaysForJob;

                if (!buildingTeamMappingService.Any(model.BuildingId, model.TeamId))
                {
                    await buildingTeamMappingService.AddAsync(model.BuildingId, model.TeamId);
                }

                var newMaterials = model.Materials
                    .Where(x => x.Quantity > 0)
                    .ToDictionary(x => x.MaterialId, x => x.Quantity);

                var allMaterialIds = newMaterials.Keys
                    .Union(oldMaterialsWithPrices.Keys)
                    .ToList();

                var newMaterialsWithPrices = new Dictionary<Guid, (decimal Quantity, decimal Price)>();

                foreach (var materialId in allMaterialIds)
                {
                    var newQty = newMaterials.ContainsKey(materialId)
                        ? newMaterials[materialId]
                        : 0m;

                    var oldData = oldMaterialsWithPrices.ContainsKey(materialId)
                        ? oldMaterialsWithPrices[materialId]
                        : (0m, 0m);

                    var oldQty = oldData.Item1;

                    decimal price = oldMaterialsWithPrices.ContainsKey(materialId)
                        ? oldData.Item2
                        : await priceService.GetPriceAsync(model.BuildingId, materialId) ?? 0m;

                    var delta = newQty - oldQty;

                    if (delta > 0)
                        await buildingMaterialService.DecreaseAsync(model.BuildingId, materialId, delta);
                    else if (delta < 0)
                        await buildingMaterialService.IncreaseAsync(model.BuildingId, materialId, Math.Abs(delta));

                    var existing = oldMappings.FirstOrDefault(x => x.MaterialId == materialId);

                    if (existing != null)
                    {
                        if (newQty == 0)
                        {
                            await materialMappingService.RemoveByIdsAsync(model.Id, materialId);
                        }
                        else
                        {
                            await materialMappingService.UpdateQuantityAsync(model.Id, materialId, newQty);
                        }
                    }
                    else if (newQty > 0)
                    {
                        await materialMappingService.AddAsync(
                            model.Id.ToString(),
                            materialId.ToString(),
                            newQty,
                            price);
                    }

                    if (newQty > 0)
                        newMaterialsWithPrices[materialId] = (newQty, price);
                }

                await earningsService.CalculateMoneyAsync(
                    model.TeamId,
                    model.Id,
                    model.DaysForJob,
                    newMaterialsWithPrices,
                    Adding);

                await jobDoneRepository.SaveAsync();

                return true;
            });
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

                var model = AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);

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

            var model = AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);

            var jobDoneTeamMappingService =
                serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            var mapping = await jobDoneTeamMappingService.GetByJobDoneIdAsync(model.Id);

            if (mapping != null)
            {
                model.TeamId = mapping.TeamId;
                model.Team = mapping.Team;
            }

            var materials = await materialMappingService
                .GetAllAttached()
                .Include(x => x.Material)
                .Where(x => x.JobDoneId == validId)
                .ToListAsync();

            decimal total = 0;

            model.Materials = materials
                .Select(x =>
                {
                    return new MaterialInputPair
                    {
                        MaterialId = x.MaterialId,
                        Quantity = x.Quantity,
                        MaterialName = x.Material.Name
                    };
                })
                .ToList();

            return model;
        }

        /// <summary>
        /// Soft deletes a job done record and restores consumed materials.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(string id, string teamId)
        {
            return await transactionService.ExecuteAsync(async () =>
            {
                Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

                var entity = await jobDoneRepository
                    .GetAllAttached()
                    .FirstOrDefaultAsync(x => x.Id == validId)
                    ?? throw new InvalidOperationException("JobDone record not found.");

                if (entity.IsDeleted)
                    throw new InvalidOperationException("JobDone already deleted.");

                var materialMappingService =
                    serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

                var buildingMaterialService =
                    serviceProvider.GetRequiredService<IBuildingMaterialMappingService>();

                var helpMethodService =
                    serviceProvider.GetRequiredService<IHelpMethodsService>();

                var mappings = await materialMappingService.GetByJobDoneIdAsync(id);

                var materialsWithPrices =
                    await helpMethodService.GetMaterialWhitQuantityAndPrice(entity.Materials, entity.BuildingId);

                await earningsCalculationService.CalculateMoneyAsync(
                    Guid.Parse(teamId),
                    validId,
                    entity.DaysForJob,
                    materialsWithPrices,
                    Remove);

                foreach (var m in mappings)
                {
                    await buildingMaterialService.IncreaseAsync(
                        entity.BuildingId,
                        m.MaterialId,
                        m.Quantity);
                }

                await materialMappingService.RemoveByJobDoneIdAsync(id);

                entity.IsDeleted = true;
                entity.DeletedDate = DateTime.UtcNow;

                await jobDoneRepository.SaveAsync();

                return true;
            });
        }

        /// <summary>
        /// Returns all non-deleted job done records.
        /// </summary>
        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted);
    }
}