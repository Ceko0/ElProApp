namespace ElProApp.Data.Configuration.Mapping
{
    using Models.Mappings;
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
            builder.HasKey(et => new {et.EmployeeId, et.TeamId});

            // Configures the relationship between Employee and EmployeeTeamMapping.
            builder
                .HasOne(et => et.Employee) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction); 

            // Configures the relationship between Team and EmployeeTeamMapping.
            builder
                .HasOne(et => et.Team) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
