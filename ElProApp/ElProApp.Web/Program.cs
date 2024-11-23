namespace ElProApp.Web
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Services.Mapping;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data;
    using ElProApp.Web.Models;
    using ElProApp.Data.Models;
    using ElProApp.Web.Infrastructure.Extensions;
    using ElProApp.Services.Data;
    using static ElProApp.Web.Infrastructure.Extensions.ApplicationBuilderExtensions;

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


            // Register repositories and services
            builder.Services.RegisterRepositories(typeof(Employee).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(EmployeeService).Assembly);

            // Add services to the container for MVC and Razor Pages
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Build the application
            var app = builder.Build();

            // Configure AutoMapper mappings
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                // Use the error handling page for non-development environments
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enable HTTP Strict Transport Security
            }

            // Enable HTTPS redirection and static file serving
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Configure request routing
            app.UseRouting();

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.SeedAdmin("admin@abv.bg", "admin", "admin@");

            // Map controller routes and Razor Pages
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Optional: Show developer exception page in development
            app.UseDeveloperExceptionPage();

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
