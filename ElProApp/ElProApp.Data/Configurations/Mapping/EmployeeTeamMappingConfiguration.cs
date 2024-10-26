namespace ElProApp.Data.Configuration.Mapping
{
    using ElProApp.Data.Models.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Configuration class for the EmployeeTeamMapping entity,
    /// defining the schema for the EmployeeTeamMapping table.
    /// </summary>
    public class EmployeeTeamMappingConfiguration : IEntityTypeConfiguration<EmployeeTeamMapping>
    {
        /// <summary>
        /// Configures the EmployeeTeamMapping entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the EmployeeTeamMapping entity.</param>
        public void Configure(EntityTypeBuilder<EmployeeTeamMapping> builder)
        {
            // Sets a composite primary key for the EmployeeTeamMapping entity.
            builder.HasKey(et => new { et.EmployeeId, et.TeamId });

            // Configures the relationship between Employee and EmployeeTeamMapping.
            builder
                .HasOne(et => et.Employee) // Each EmployeeTeamMapping has one Employee
                .WithMany(e => e.TeamsEmployeeBelongsTo) // An Employee can be part of many EmployeeTeamMappings
                .HasForeignKey(et => et.EmployeeId) // Foreign key defined in EmployeeTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship

            // Configures the relationship between Team and EmployeeTeamMapping.
            builder
                .HasOne(et => et.Team) // Each EmployeeTeamMapping has one Team
                .WithMany(t => t.EmployeesInTeam) // A Team can have many EmployeeTeamMappings
                .HasForeignKey(et => et.TeamId) // Foreign key defined in EmployeeTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship
        }
    }
}
