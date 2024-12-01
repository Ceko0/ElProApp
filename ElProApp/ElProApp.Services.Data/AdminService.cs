namespace ElProApp.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Data;

    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Web.Models.Admin;

    public class AdminService(IServiceProvider _serviceProvider,
            UserManager<IdentityUser> _userManager,
            RoleManager<IdentityRole> _roleManager) : IAdminService
    {
        private readonly IServiceProvider serviceProvider = _serviceProvider;
        private readonly UserManager<IdentityUser> userManager = _userManager;
        private readonly RoleManager<IdentityRole> roleManager = _roleManager;

        public async Task<List<UserRolesViewModel>> GetUsersRolesAsync()
        {
            var users = userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }
            return userRolesViewModel;
        }

        public async Task<List<string>> GetAllRolesAsync()
            => await roleManager.Roles.Select(r => r.Name).ToListAsync();

        public async Task<bool> PostUsersRolesAsync(string userId, List<string> roles, string state)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            switch (state)
            {
                case "add":
                    var rolesToAdd = roles.Except(currentRoles).ToList();
                    await userManager.AddToRolesAsync(user, roles);
                    break;
                case "remove":
                    var rolesToRemove = currentRoles.Except(roles).ToList();
                    await userManager.RemoveFromRolesAsync(user, roles);
                    break;
            }
            return true;
        }
        
    }
}
