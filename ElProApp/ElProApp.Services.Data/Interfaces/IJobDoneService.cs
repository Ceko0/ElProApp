using ElProApp.Data.Models;
using ElProApp.Web.Models.JobDone;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IJobDoneService
    {
        Task<JobDoneInputModel> AddAsync();
        Task<string> AddAsync(JobDoneInputModel model);
        Task<JobDoneViewModel> GetByIdAsync(string id);
        Task<JobDoneEditInputModel> EditByIdAsync(string id);
        Task<bool> EditByModelAsync(JobDoneEditInputModel model);
        Task<ICollection<JobDoneViewModel>> GetAllAsync();
        IQueryable<JobDone> GetAllAttached();
        Task<bool> SoftDeleteAsync(string id);
    }
}
