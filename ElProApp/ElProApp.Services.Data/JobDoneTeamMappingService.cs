namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.JobDone;
    using Microsoft.EntityFrameworkCore;
    using System.Xml.Linq;

    public class JobDoneTeamMappingService(IRepository<JobDoneTeamMapping, Guid> _jobDoneTeamMappingRepository) : IJobDoneTeamMappingService
    {
        private readonly IRepository<JobDoneTeamMapping, Guid> jobDoneTeamMappingRepository = _jobDoneTeamMappingRepository;
     
        public async Task<ICollection<JobDoneTeamMapping>> GetAllAsync()
            => await jobDoneTeamMappingRepository
                .GetAllAttached()                
                .ToListAsync();

        public IQueryable<JobDoneTeamMapping> GetAllAttached()
            => jobDoneTeamMappingRepository
                .GetAllAttached();

        public async Task<ICollection<JobDoneTeamMapping>> GetByTeamIdAsync(Guid Id)
            => await jobDoneTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == Id)
                .ToListAsync();               
                
        public async Task<JobDoneTeamMapping> AddAsync(Guid jobDoneId, Guid teamId)
        {
            var model = new JobDoneTeamMapping()
            {
                Id = Guid.NewGuid(),
                JobDoneId = jobDoneId,
                TeamId = teamId
            };

            await jobDoneTeamMappingRepository.AddAsync(model);

            return model;
        }

        public bool Any(Guid jobDoneId, Guid teamId)
        {
            var model = jobDoneTeamMappingRepository.GetAllAttached().Where(x => x.JobDoneId == jobDoneId && x.TeamId == teamId);

            return model.Any();
        }

        public async Task<bool> RemoveAsync(JobDoneTeamMapping mapping) 
            => await jobDoneTeamMappingRepository.DeleteByCompositeKeyAsync(mapping.JobDoneId, mapping.TeamId);

    }
}
