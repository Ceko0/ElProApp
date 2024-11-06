namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.JobDone;

    public class JobDoneService(IRepository<JobDone, Guid> _jobDoneRepository) : IJobDoneService
    {
        private readonly IRepository<JobDone, Guid> jobDoneRepository = _jobDoneRepository;

        public async Task<string> AddAsync(JobDoneInputModel model)
        {
            var jobDone = AutoMapperConfig.MapperInstance.Map<JobDone>(model);

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

        public async Task<IEnumerable<JobDoneViewModel>> GetAllAsync()
        {
            var jobDone = await jobDoneRepository.GetAllAttached()
                                   .Include(x => x.TeamsDoTheJob)
                                   .ThenInclude(tj => tj.Team)
                                   .To<JobDoneViewModel>()
                                   .ToListAsync();
            return (jobDone);
        }

        public async Task<JobDoneViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await jobDoneRepository.GetByIdAsync(validId).ConfigureAwait(false);
            return entity != null
                ? AutoMapperConfig.MapperInstance.Map<JobDoneViewModel>(entity)
                : throw new ArgumentException("Missing entity.");
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
