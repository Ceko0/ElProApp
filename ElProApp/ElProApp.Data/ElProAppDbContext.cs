namespace ElProApp.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;
    using System.Linq.Expressions;

    using Models.Mappings;
    using Models;
    using ElProApp.Services.Data.Interfaces;

    /// <summary>
    /// Represents the database context for the ElProApp application, which manages database sets for the app's entities.
    /// </summary>
    public class ElProAppDbContext(DbContextOptions<ElProAppDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        /// <summary>
        /// DbSet for managing buildings in the database.
        /// </summary>
        public DbSet<Building> Buildings { get; set; }

        /// <summary>
        /// DbSet for managing employees in the database.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// DbSet for managing jobs in the database.
        /// </summary>
        public DbSet<Job> Jobs { get; set; }

        /// <summary>
        /// DbSet for managing records of completed jobs in the database.
        /// </summary>
        public DbSet<JobDone> JobsDone { get; set; }

        /// <summary>
        /// DbSet for managing teams in the database.
        /// </summary>
        public DbSet<Team> Teams { get; set; }

        /// <summary>
        /// DbSet for managing materials in the database.
        /// </summary>
        public DbSet<Material> Materials { get; set; }

        /// <summary>
        /// DbSet for managing mappings between buildings and teams in the database.
        /// </summary>
        public DbSet<BuildingTeamMapping> BuildingTeamMappings { get; set; }

        /// <summary>
        /// DbSet for managing mappings between employees and teams in the database.
        /// </summary>
        public DbSet<EmployeeTeamMapping> EmployeeTeamMappings { get; set; }

        /// <summary>
        /// DbSet for managing mappings between completed jobs and teams in the database.
        /// </summary>
        public DbSet<JobDoneTeamMapping> JobDoneTeamMappings { get; set; }

        /// <summary>
        /// DbSet for managing mappings between JobDone and Material in the database.
        /// </summary>
        public DbSet<JobDoneMaterialMapping> JobDoneMaterialMappings { get; set; }

        /// <summary>
        /// DbSet for managing mappings between Building and Material in the database.
        /// </summary>
        public DbSet<BuildingMaterialMapping> BuildingMaterialMappings { get; set; }

        public DbSet<JobMaterialMapping> JobMaterialMappings { get; set; }

        /// <summary>
        /// Configures the entity mappings using the configurations defined in the current assembly.
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API surface for configuring a `DbContext` model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var softDeletableEntities = modelBuilder.Model
                .GetEntityTypes()
                .Where(et => typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));

            foreach (var entityType in softDeletableEntities)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateIsDeletedRestriction(entityType.ClrType));
            }
        }

        private static LambdaExpression CreateIsDeletedRestriction(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var prop = Expression.Property(param, nameof(IDeletableEntity.IsDeleted));
            var body = Expression.Equal(prop, Expression.Constant(false));

            return Expression.Lambda(body, param);
        }
    }
}