using ElProApp.Web.Models.Team;

namespace ElProApp.Services.Data.Interfaces
{
    public interface ITeamService
    {
        Task<TeamInputModel> AddAsync();
        Task<string> AddAsync(TeamInputModel model);

        Task<TeamViewModel> GetByIdAsync(string id);

        Task<TeamEditInputModel> EditByIdAsync(string id);

        Task<bool> EditByModelAsync(TeamEditInputModel model);

        Task<IEnumerable<TeamViewModel>> GetAllAsync();
        Task<bool> SoftDeleteAsync(string id);
    }
}
