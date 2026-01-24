namespace ElProApp.Services.Data.Interfaces
{
    public interface IMaterialConsumptionService
    {
        Task ApplyAsync(
            Guid jobDoneId,
            Guid buildingId,
            Dictionary<Guid, decimal> materials
        );

        Task RollbackAsync(Guid jobDoneId);
    }

}
