namespace ElProApp.Web.Infrastructure.Extensions
{
    using System.Reflection;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Data.Models;
    using Data.Repository;
    using Data.Repository.Interfaces;

    /// <summary>
    /// Extension methods for configuring service collections in ASP.NET Core.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all repositories based on the provided assembly containing model types.
        /// </summary>
        /// <param name="services">The service collection to which the repositories will be added.</param>
        /// <param name="modelsAssembly">The assembly containing the model types.</param>
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly)
        {
            // Types to exclude from repository registration.
            Type[] typesToExclude = { typeof(IdentityUser) };

            // Retrieve model types from the assembly that are not abstract or interfaces.
            Type[] modelTypes = modelsAssembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                            !t.Name.ToLower().EndsWith("attribute"))
                .ToArray();

            // Register each model type with its corresponding repository interface and implementation.
            foreach (Type type in modelTypes)
            {
                if (!typesToExclude.Contains(type))
                {
                    Type repositoryInterface = typeof(IRepository<,>);
                    Type repositoryInstanceType = typeof(BaseRepository<,>);

                    // Find the ID property for the model type.
                    PropertyInfo? idPropInfo = type
                        .GetProperties()
                        .Where(p => p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                        .SingleOrDefault();

                    // Prepare constructor arguments for the repository types.
                    Type[] constructArgs = new Type[2];
                    constructArgs[0] = type; // Model type

                    // Set the ID type based on the presence of the ID property.
                    if (idPropInfo == null)
                    {
                        constructArgs[1] = typeof(object); // Default to object if ID property is not found.
                    }
                    else
                    {
                        constructArgs[1] = idPropInfo.PropertyType; // Use the type of the ID property.
                    }

                    // Create the generic repository interface and instance types.
                    repositoryInterface = repositoryInterface.MakeGenericType(constructArgs);
                    repositoryInstanceType = repositoryInstanceType.MakeGenericType(constructArgs);

                    // Register the repository in the service collection.
                    services.AddScoped(repositoryInterface, repositoryInstanceType);
                }
            }
        }

        /// <summary>
        /// Registers user-defined services based on the provided assembly.
        /// </summary>
        /// <param name="services">The service collection to which the services will be added.</param>
        /// <param name="serviceAssembly">The assembly containing the service types.</param>
        public static void RegisterUserDefinedServices(this IServiceCollection services, Assembly serviceAssembly)
        {
            // Retrieve all service interfaces from the assembly.
            Type[] serviceInterfaceTypes = serviceAssembly
                .GetTypes()
                .Where(t => t.IsInterface)
                .ToArray();

            // Retrieve all concrete service types ending with "Service".
            Type[] serviceTypes = serviceAssembly
                .GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract &&
                            t.Name.ToLower().EndsWith("service"))
                .ToArray();

            // Register each service interface with its corresponding implementation.
            foreach (Type serviceInterfaceType in serviceInterfaceTypes)
            {
                Type? serviceType = serviceTypes
                    .SingleOrDefault(t => ("i" + t.Name.ToLower()).Equals(serviceInterfaceType.Name, StringComparison.CurrentCultureIgnoreCase))
                    ?? throw new NullReferenceException($"Service type could not be obtained for the service {serviceInterfaceType.Name}");

                // Register the service in the service collection.
                services.AddScoped(serviceInterfaceType, serviceType);
            }
        }
    }
}
