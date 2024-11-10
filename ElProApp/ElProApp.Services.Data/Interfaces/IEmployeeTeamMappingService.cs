namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IEmployeeTeamMappingService
    {
        ICollection<EmployeeTeamMapping> GetAllByEmployeeId(string id);
        Task<ICollection<EmployeeTeamMapping>> GetAllAsync();
        IQueryable<EmployeeTeamMapping> GetAllAttached();
        Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid TeamId);
        Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid id);
        bool Any(Guid employeeId, Guid teamId);
        Task<bool> RemoveAsync(EmployeeTeamMapping mapping);
    }
}
