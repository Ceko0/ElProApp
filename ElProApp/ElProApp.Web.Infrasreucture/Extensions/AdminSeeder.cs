namespace ElProApp.Web.Infrastructure.Extensions
{
    using ElProApp.Data.Models;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using static ElProApp.Common.ApplicationConstants;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedAdmin(this IApplicationBuilder app, string Email, string UserName, string Password)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateAsyncScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;

            RoleManager<IdentityRole>? roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null) throw new ArgumentNullException(nameof(roleManager),
                    $"cannot create {typeof(RoleManager<IdentityRole>)}");

            IUserStore<IdentityUser>? userStore = serviceProvider.GetService<IUserStore<IdentityUser>>();
            if (userStore == null) throw new ArgumentNullException(nameof(userStore),
                    $"cannot create {typeof(UserStore<IdentityUser>)}");

            UserManager<IdentityUser>? userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            if (userManager == null) throw new ArgumentNullException(nameof(userManager),
                    $"cannot create {typeof(UserManager<IdentityRole>)}");

            var employeeService = serviceProvider.GetService<IEmployeeService>();

            Task.Run(async() =>
            {
                bool roleExist = await roleManager.RoleExistsAsync(AdminRoleName);
                if (roleExist)
                {
                    return app;
                }

                IdentityRole adminRole = new IdentityRole(AdminRoleName);

                var result = await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded) throw new InvalidOperationException($"Failed to create the {AdminRoleName} role");

                var adminUser = await userStore.FindByNameAsync(UserName, CancellationToken.None);
                if (adminUser == null) adminUser = await CreateAdminUserAsync(Email, UserName, Password, userStore, userManager,employeeService);

                result = await userManager.AddToRoleAsync(adminUser, AdminRoleName);
                if (!result.Succeeded) throw new InvalidOperationException($"Failed to adding {UserName} to the {AdminRoleName} role");
                return app;
            })
                .GetAwaiter()
                .GetResult();
           
            return app;
        }

        private static async Task<IdentityUser> CreateAdminUserAsync(string Email, string UserName , string Password ,IUserStore<IdentityUser> userStore,UserManager<IdentityUser> userManager,IEmployeeService employeeService)
        {
            IdentityUser AdminUser= new IdentityUser
            {
                Email = Email
            };
            
            await userStore.SetUserNameAsync(AdminUser , UserName ,CancellationToken.None);
            var result = await userManager.CreateAsync(AdminUser,Password);
            if (!result.Succeeded) throw new InvalidOperationException($"Failed to create {nameof(IdentityUser)}");

            var identityUserId = await userManager.GetUserIdAsync(AdminUser);

            var employeeId = await employeeService.AddAdminEmployeeAsync(UserName, UserName, identityUserId.ToString());
            if(employeeId == null) throw new InvalidOperationException($"Failed to create {nameof(Employee)}");
            return AdminUser;
        }
    }

}
