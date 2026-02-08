namespace ElProApp.Application.Services.Interfaces
{
    using ElProApp.Data.Models.Mappings;

    public interface IBuildingMaterialMappingService
    {
        Task<bool> HasEnoughAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        Task DecreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        Task IncreaseAsync(
            Guid buildingId,
            Guid materialId,
            decimal quantity);

        IQueryable<BuildingMaterialMapping> GetAllAttached();
    }

}
