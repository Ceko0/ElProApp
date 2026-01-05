namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Building;
    using Web.Models.Material;

    public interface IMaterialService
    {

        public Task<MaterialInputModel> GetAddModelAsync();

        public Task<string> AddAsync(MaterialInputModel model);

        public Task<MaterialEditInputModel> GetEditByIdAsync(string id);

        public Task<ICollection<MaterialViewModel>> GetAllAsync();

        public IQueryable<Material> GetAllAttached();

        public Task<MaterialViewModel?> GetByIdAsync(string id);

        public Task<bool> SoftDeleteAsync(string id);

        public Task<bool> EditByModelAsync(MaterialEditInputModel model);
    }
}
