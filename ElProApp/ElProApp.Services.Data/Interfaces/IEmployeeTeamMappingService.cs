namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IEmployeeTeamMappingService
    {
        public ICollection<EmployeeTeamMapping> GetAllByEmployeeId(string id);
        public Task<ICollection<EmployeeTeamMapping>> GetAllAsync();
        public Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid TeamId);
        public Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid id);
        public bool Any(Guid employeeId, Guid teamId);
        public Task<bool> RemoveAsync(EmployeeTeamMapping mapping);
    }
}
