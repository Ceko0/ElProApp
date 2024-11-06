namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IEmployeeTeamMappingService
    {
        public IEnumerable<EmployeeTeamMapping> GetAllByEmployeeId(string id);
        public Task<IEnumerable<EmployeeTeamMapping>> GetAllAsync();
        public Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid TeamId);
        public IEnumerable<EmployeeTeamMapping> GetAllByTeamId(Guid id);
    }
}
