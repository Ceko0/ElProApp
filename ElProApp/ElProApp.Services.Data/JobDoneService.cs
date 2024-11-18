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

    public class JobDoneService(IRepository<JobDone, Guid> _jobDoneRepository
        , IServiceProvider _serviceProvider)
        : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository = _jobDoneRepository;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        public async Task<JobDoneInputModel> AddAsync()
        {
            var TeamService = serviceProvider.GetRequiredService<ITeamService>();
            var jobService = serviceProvider.GetRequiredService<IJobService>();

            var model = new JobDoneInputModel();
            model.teams = await TeamService.GetAllAttached().ToListAsync();
            model.jobs = await jobService.GetAllAttached().ToListAsync();
            return model;
        }
        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            var jobDoneTeamMppingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

            await jobDoneTeamMppingService.AddAsync(model.Id, model.TeamId);

            var jobDone = AutoMapperConfig.MapperInstance.Map<JobDone>(model);

            var JobService = serviceProvider.GetRequiredService<IJobService>();
            var currentJob = await JobService.GetAllAttached().Include(x => x.JobsDone).FirstOrDefaultAsync(x => x.Id == model.JobId);
            currentJob?.JobsDone.Append(jobDone);

            await jobDoneRepository.AddAsync(jobDone);
            return jobDone.Id.ToString();
        }

        public async Task<JobDoneEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await jobDoneRepository.GetByIdAsync(validId);

            return AutoMapperConfig.MapperInstance.Map<JobDoneEditInputModel>(entity);
        }

        public async Task<bool> EditByModelAsync(JobDoneEditInputModel model)
        {
            try
            {
                var entity = await jobDoneRepository.GetByIdAsync(model.Id);
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await jobDoneRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ICollection<JobDoneViewModel>> GetAllAsync()
            => await jobDoneRepository.GetAllAttached()
                .To<JobDoneViewModel>()
                .ToListAsync();

        public IQueryable<JobDone> GetAllAttached()
            => jobDoneRepository
            .GetAllAttached();

        public async Task<JobDoneViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await jobDoneRepository.GetAllAttached().Include(x => x.Job).FirstOrDefaultAsync(x => x.Id == validId);

            if (entity != null)
            {
                JobDoneViewModel? model = AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity);
                return model;
            }

            throw new ArgumentException("Missing entity.");
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await jobDoneRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch
            {
                return false;
            }
        }

        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }
    }
}
