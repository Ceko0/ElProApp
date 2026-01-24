using ElProApp.Data.Models.Mappings;

public interface IJobDoneMaterialMappingService
{
    Task<JobDoneMaterialMapping> AddAsync(string jobDoneId, string materialId, decimal quantity, decimal unitPrice);

    Task<ICollection<JobDoneMaterialMapping>> GetByJobDoneIdAsync(string jobDoneId);

    Task<ICollection<JobDoneMaterialMapping>> GetByMaterialIdAsync(string materialId);

    Task<ICollection<JobDoneMaterialMapping>> GetAllAttachedAsync();

    IQueryable<JobDoneMaterialMapping> GetAllAttached();

    bool Any(string jobDoneId, string materialId);

    Task<bool> RemoveAsync(JobDoneMaterialMapping mapping);
    Task RemoveByJobDoneIdAsync(string jobDoneId);
}
