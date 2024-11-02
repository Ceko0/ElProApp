using ElProApp.Web.Models.Team;

namespace ElProApp.Services.Data.Interfaces
{
    public interface ITeamService
    {
        Task<string> AddAsync(TeamAddInputModel model);

        Task<TeamViewModel> GetByIdAsync(string id);

        Task<TeamEditInputModel> EditByIdAsync(string id);

        Task<bool> EditByModelAsync(TeamEditInputModel model);

        Task<IEnumerable<TeamAllViewModel>> GetAllAsync();
        Task<bool> SoftDeleteAsync(string id);



    }
}
