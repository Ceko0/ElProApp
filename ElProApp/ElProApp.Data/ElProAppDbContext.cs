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
    /// Represents the database context for the application and manages all entity sets.
    /// </summary>
    public class ElProAppDbContext(DbContextOptions<ElProAppDbContext> options)
        : IdentityDbContext<IdentityUser>(options)
    {
        /// <summary>
        /// Gets or sets buildings.
        /// </summary>
        public DbSet<Building> Buildings { get; set; }

        /// <summary>
        /// Gets or sets employees.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }
                
        /// <summary>
        /// Gets or sets job-done records.
        /// </summary>
        public DbSet<JobDone> JobsDone { get; set; }

        /// <summary>
        /// Gets or sets teams.
        /// </summary>
        public DbSet<Team> Teams { get; set; }

        /// <summary>
        /// Gets or sets materials.
        /// </summary>
        public DbSet<Material> Materials { get; set; }

        /// <summary>
        /// Gets or sets building-team mappings.
        /// </summary>
        public DbSet<BuildingTeamMapping> BuildingTeamMappings { get; set; }

        /// <summary>
        /// Gets or sets employee-team mappings.
        /// </summary>
        public DbSet<EmployeeTeamMapping> EmployeeTeamMappings { get; set; }

        /// <summary>
        /// Gets or sets job-done team mappings.
        /// </summary>
        public DbSet<JobDoneTeamMapping> JobDoneTeamMappings { get; set; }

        /// <summary>
        /// Gets or sets job-done material mappings.
        /// </summary>
        public DbSet<JobDoneMaterialMapping> JobDoneMaterialMappings { get; set; }

        /// <summary>
        /// Gets or sets building-material quantity mappings.
        /// </summary>
        public DbSet<BuildingMaterialMapping> BuildingMaterialMappings { get; set; }

        /// <summary>
        /// Gets or sets building-material price mappings.
        /// </summary>
        public DbSet<BuildingMaterialPrice> BuildingMaterialPrices { get; set; }

        /// <summary>
        /// Configures the entity mappings and global query filters.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
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

        /// <summary>
        /// Creates a global query filter for non-deleted entities.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The filter expression.</returns>
        private static LambdaExpression CreateIsDeletedRestriction(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var prop = Expression.Property(param, nameof(IDeletableEntity.IsDeleted));
            var body = Expression.Equal(prop, Expression.Constant(false));

            return Expression.Lambda(body, param);
        }
    }
}