namespace ElProApp.Data.Configuration.Mapping
{
    using ElProApp.Data.Models.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Configuration class for the JobDoneTeamMapping entity,
    /// defining the schema for the JobDoneTeamMapping table.
    /// </summary>
    public class JobDoneTeamMappingConfiguration : IEntityTypeConfiguration<JobDoneTeamMapping>
    {
        /// <summary>
        /// Configures the JobDoneTeamMapping entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the JobDoneTeamMapping entity.</param>
        public void Configure(EntityTypeBuilder<JobDoneTeamMapping> builder)
        {
            // Sets the primary key for the JobDoneTeamMapping entity.
            builder.HasKey(et => new { et.JobDoneId, et.TeamId });

            // Configures the relationship between JobDone and JobDoneTeamMapping.
            builder
                .HasOne(jdtm => jdtm.JobDone) // Each JobDoneTeamMapping has one JobDone
                .WithMany() // A JobDone can have many JobDoneTeamMappings
                .HasForeignKey(jdtm => jdtm.JobDoneId) // Foreign key defined in JobDoneTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship

            // Configures the relationship between Team and JobDoneTeamMapping.
            builder
                .HasOne(jdtm => jdtm.Team) // Each JobDoneTeamMapping has one Team
                .WithMany() // A Team can have many JobDoneTeamMappings
                .HasForeignKey(jdtm => jdtm.TeamId) // Foreign key defined in JobDoneTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship
        }
    }
}
