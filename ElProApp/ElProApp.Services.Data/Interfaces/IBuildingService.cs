namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Building;

    public interface IBuildingService
    {
        Task<string> AddAsync(BuildingInputModel model);
        BuildingViewModel GetById(string id);
        Task<BuildingEditInputModel> GetEditByIdAsync(string id);
        Task<bool> EditByModelAsync(BuildingEditInputModel model);
        Task<ICollection<BuildingViewModel>> GetAllAsync();
        IQueryable<Building> GetAllAttached();
        Task<bool> SoftDeleteAsync(string id);
    }
}
