namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Job;
    using ElProApp.Web.Models.JobDone;
    using Microsoft.EntityFrameworkCore;

    public class JobService(IRepository<Job, Guid> _jobRepository) : IJobService
    {
        private readonly IRepository<Job, Guid> jobRepository = _jobRepository;
       
        public async Task<string> AddAsync(JobInputModel model)
        {
            var entity = AutoMapperConfig.MapperInstance.Map<Job>(model);

            await jobRepository.AddAsync(entity);
            return entity.Id.ToString();
        }

        public async Task<JobViewModel> GetByIdAsync(string id)
        {
            Guid ValidId = ConvertAndTestIdToGuid(id);

            var entity = await jobRepository.GetByIdAsync(ValidId);

            if (entity != null)
            {
                var model = AutoMapperConfig.MapperInstance.Map<JobViewModel>(entity);

                return model;
            }

            throw new ArgumentException("Missing entity.");
        }

        public async Task<JobEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await jobRepository.GetByIdAsync(validId);

            return AutoMapperConfig.MapperInstance.Map<JobEditInputModel>(entity);
            
        }

        public async Task<bool> EditByModelAsync(JobEditInputModel model)
        {
            try
            {
                var entity = await jobRepository.GetByIdAsync(model.Id);
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await jobRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ICollection<JobViewModel>> GetAllAsync() => await jobRepository.GetAllAttached()
            .To<JobViewModel>()
            .ToListAsync();

        public IQueryable<Job> GetAllAttached()
           => jobRepository
           .GetAllAttached();

        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await jobRepository.SoftDeleteAsync(validId);
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
