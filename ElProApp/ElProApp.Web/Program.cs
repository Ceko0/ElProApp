namespace ElProApp.Web
{
    using Services.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Data.Models;
    using ElProApp.Services.Data;
    using Data;
    using Models;
    using Infrastructure.Extensions;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a new web application builder
            var builder = WebApplication.CreateBuilder(args);

            // Configure the database context to use SQL Server
            builder.Services.AddDbContext<ElProAppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

            // Configure default identity settings
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Настройка на опциите за Identity
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<ElProAppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.LoginPath = new PathString("/Identity/Account/Login");
            });


            // Register repositories and services
            builder.Services.RegisterRepositories(typeof(Employee).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(EmployeeService).Assembly);

            // Add services to the container for MVC and Razor Pages
            builder.Services.AddControllersWithViews(cfg =>
            {
                cfg.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddRazorPages();

            // Build the application
            var app = builder.Build();
            
            // Configure AutoMapper mappings
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enable HTTP Strict Transport Security
            }
            
            // Enable HTTPS redirection and static file serving
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Configure request routing
            app.UseRouting();
            app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            Task<IApplicationBuilder> seedAdmin = app.SeedAdminAndRoles("admin@abv.bg", "admin", "admin@");

            // Map controller routes and Razor Pages
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
            
            // Seed the database with initial data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ElProAppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var seeder = new Data.SeedData.DatabaseSeeder(dbContext, userManager);
                seeder.SeedDatabase();
            }

            // Run the application
            app.Run();
        }
    }
}
