using ElProApp.Data.Models.Mappings;
using ElProApp.Web.Models.JobDone;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IJobDoneTeamMappingService
    {
        public Task<ICollection<JobDoneTeamMapping>> GetAllAsync();
        public Task<ICollection<JobDoneTeamMapping>> GetByTeamIdAsync(Guid Id);
        public Task<JobDoneTeamMapping> AddAsync(Guid jobDoneID, Guid teamId);
        public bool Any(Guid jobDoneId, Guid teamId);
        public Task<bool> RemoveAsync(JobDoneTeamMapping mapping);
    }
}
