namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Web.Models.Building;

    public interface IBuildingService
    {
        Task<string> AddAsync(BuildingInputModel model);

        Task<BuildingViewModel> GetByIdAsync(string id);

        Task<BuildingEditInputModel> EditByIdAsync(string id);

        Task<bool> EditByModelAsync(BuildingEditInputModel model);

        Task<ICollection<BuildingViewModel>> GetAllAsync();
        Task<bool> SoftDeleteAsync(string id);
    }
}
