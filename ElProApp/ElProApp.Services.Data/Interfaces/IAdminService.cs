namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Web.Models.Admin;

    public interface IAdminService 
    {
        public Task<List<UserRolesViewModel>> GetUsersRolesAsync();
        public Task<List<string>> GetAllRolesAsync();
        public Task<bool> PostUsersRolesAsync(string userId, List<string> roles, string state);
    }
}
