namespace ElProApp.Web
{
    using Services.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Data.Models;
    using ElProApp.Application.Services;
    using Data;
    using Models;
    using Infrastructure.Extensions;
    using ElProApp.Web.Areas.Identity.Pages.Account;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ElProAppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("ElProAppDbContextConnection"),
                    new MySqlServerVersion(new Version(5, 7, 44))));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<ElProAppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.LoginPath = new PathString("/Identity/Account/Login");
                options.AccessDeniedPath = new PathString("/Home/Error/403");
            });

            builder.Services.RegisterRepositories(typeof(Employee).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(EmployeeService).Assembly);
            builder.Services.AddScoped<Infrastructure.Interfaces.IAdminService,Infrastructure.Services.AdminService>();
            builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,EmailSender>();

            builder.Services.AddControllersWithViews(cfg =>
            {
                cfg.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddRazorPages();

            var app = builder.Build();
            
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseAuthentication();
            app.UseAuthorization();

            Task<IApplicationBuilder> seedAdmin = app.SeedAdminAndRoles("admin@abv.bg", "admin", "admin@");

            app.MapControllerRoute(
               name: "Areas",
               pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Error",
                pattern: "Home/Error/{statusCode?}",
                defaults: new { controller = "Home", action = "Error" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ElProAppDbContext>();
            //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //    var seeder = new Data.SeedData.DatabaseSeeder(dbContext, userManager);
            //    seeder.SeedDatabase();
            //}

            app.Run();
        }
    }
}
