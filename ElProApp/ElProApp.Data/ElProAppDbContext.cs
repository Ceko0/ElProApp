namespace ElProApp.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    using Models.Mappings;
    using Models;

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
        /// Configures the entity mappings using the configurations defined in the current assembly.
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API surface for configuring a `DbContext` model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies all configurations in the assembly that inherit from IEntityTypeConfiguration.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
