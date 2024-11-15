namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using Web.Models.Job;

    public interface IJobService
    {
        IQueryable<Job> GetAllAttached();
        Task<string> AddAsync(JobInputModel model);
        Task<JobViewModel> GetByIdAsync(string id);
        Task<JobEditInputModel> EditByIdAsync(string id);
        Task<bool> EditByModelAsync(JobEditInputModel model);
        Task<ICollection<JobViewModel>> GetAllAsync();
        Task<bool> SoftDeleteAsync(string id);
    }
}
