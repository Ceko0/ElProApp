using ElProApp.Data.Models;
using ElProApp.Data.Repository.Interfaces;
using ElProApp.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElProApp.Services.Data
{
    public class JobService(IRepository<Job,Guid> _jobRepository) : IJobService
    {
        private readonly IRepository<Job, Guid> jobRepository = _jobRepository;
        public IQueryable<Job> GetAllAttached() 
            => jobRepository
            .GetAllAttached();
    }
}
