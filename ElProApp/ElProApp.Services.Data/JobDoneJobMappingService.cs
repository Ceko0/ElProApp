using System.ComponentModel;

namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using Interfaces;

    public class JobDoneJobMappingService(IRepository<JobDoneJobMapping, object> JobDoneJobRepository, IHelpMethodsService helpMethodsService)
        : IJobDoneJobMappingService
    {
        public async Task<JobDoneJobMapping> AddAsync(Guid jobDoneId, Guid jobId ,decimal quantity)
        {
            var jobDone = await helpMethodsService.GetAllJobDones().FirstOrDefaultAsync(x => x.Id == jobDoneId && !x.IsDeleted);
            if (jobDone == null) throw new ArgumentNullException("jobDoneId is invalid");

            var job = await helpMethodsService.GetAllJobs().FirstOrDefaultAsync(x => x.Id == jobId && !x.IsDeleted);
            if (job == null) throw new ArgumentNullException("jobId is invalid");

            var model = new JobDoneJobMapping()
            {
                JobDoneId = jobDoneId,
                JobDone = jobDone,
                JobId = jobId,
                Job = job,
                Quantity = quantity
            };
            await JobDoneJobRepository.AddAsync(model);

            return model;
        }

        public async Task<ICollection<JobDoneJobMapping>> GetByJobIdAsync(Guid jobId)
            => await JobDoneJobRepository.GetAllAttached()
                                         .Where(x => x.JobId == jobId)
                                         .ToListAsync();
        
        public async Task<ICollection<JobDoneJobMapping>> GetByJobDoneIdAsync(Guid jobDoneId) 
            => await JobDoneJobRepository.GetAllAttached()
                                         .Where(x => x.JobDoneId == jobDoneId)
                                         .ToListAsync();

        public async Task<ICollection<JobDoneJobMapping>> GetAllAttachedAsync()
            => await JobDoneJobRepository.GetAllAttached()
                                         .Include(x => x.Job)
                                         .Include(x => x.JobDone)
                                         .ToListAsync();

        public IQueryable<JobDoneJobMapping> GetAllAttached()
            => JobDoneJobRepository.GetAllAttached();
        
        public bool Any(Guid jobDoneId, Guid jobId)
        {
            if (jobDoneId == Guid.Empty || jobId == Guid.Empty)
                throw new ArgumentException("JobDone and Job must not be empty.");

            var model = JobDoneJobRepository.GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneId && x.JobId == jobId);
            return model.Any();
        }

        public async Task<bool> RemoveAsync(Guid jobDoneId , Guid jobId)
        {
            var mappingExist = await JobDoneJobRepository
                .GetAllAttached()
                .AnyAsync(x => x.JobDoneId == jobDoneId && x.JobId == jobId);

            if (!mappingExist) throw new InvalidOperationException("mapping not found");
                
            return await JobDoneJobRepository.DeleteByCompositeKeyAsync(jobDoneId,jobId);
        }
    }
}
