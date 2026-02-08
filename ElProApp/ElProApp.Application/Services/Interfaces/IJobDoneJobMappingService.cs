namespace ElProApp.Application.Services.Interfaces
{
    using ElProApp.Data.Models.Mappings;
    public interface IJobDoneJobMappingService
    {
        Task<JobDoneJobMapping> AddAsync(Guid jobDoneId, Guid jobId , decimal quantity);

        Task<ICollection<JobDoneJobMapping>> GetByJobIdAsync(Guid jobId);
        
        Task<ICollection<JobDoneJobMapping>> GetByJobDoneIdAsync(Guid jobDoneId);
        
        Task<ICollection<JobDoneJobMapping>> GetAllAttachedAsync();

        IQueryable<JobDoneJobMapping> GetAllAttached();

        bool Any(Guid jobDoneId, Guid jobId);

        Task<bool> RemoveAsync(Guid jobDoneId, Guid jobId);
    }
}
