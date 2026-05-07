using Microsoft.Extensions.DependencyInjection;
using ElProApp.Application.Services;
using ElProApp.Application.Services.Interfaces;

namespace ElProApp.Application
{
    /// <summary>
    /// Provides dependency injection registration for the application layer.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application layer services.
        /// </summary>
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IBuildingService, BuildingService>();

            return services;
        }
    }
}
