namespace ElProApp.Data
{ 
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    using Configuration;
    using Models.Mappings;
    using Models;
    using Configuration.Mapping;

    public class ElProAppDbContext : IdentityDbContext<IdentityUser>
    {
        public ElProAppDbContext(DbContextOptions<ElProAppDbContext> option)
            : base(option)
        {
        }

        public DbSet<Building> buildings { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Job> jobs { get; set; }
        public DbSet<JobDone> jobsDone { get; set; }
        public DbSet<Team> teams { get; set; }
        public DbSet<BuildingTeamMapping> buildingTeamMappings { get; set; }
        public DbSet<EmployeeTeamMapping> employeeTeamMappings { get; set; }
        public DbSet<JobDoneTeamMapping> jobDoneTeamMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
