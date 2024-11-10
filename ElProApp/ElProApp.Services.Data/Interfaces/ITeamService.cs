using ElProApp.Data.Models;
using ElProApp.Web.Models.Team;

namespace ElProApp.Services.Data.Interfaces
{
    public interface ITeamService
    {
        Task<TeamInputModel> AddAsync();
        Task<string> AddAsync(TeamInputModel model);
        Task<TeamViewModel> GetByIdAsync(string id);
        IQueryable<Team> GetAllAttached();
        Task<TeamEditInputModel> EditByIdAsync(string id);
        Task<bool> EditByModelAsync(TeamEditInputModel model);
        Task<ICollection<TeamViewModel>> GetAllAsync();
        Task<bool> SoftDeleteAsync(string id);
        Task<bool> Any(Guid id);
    }
}
