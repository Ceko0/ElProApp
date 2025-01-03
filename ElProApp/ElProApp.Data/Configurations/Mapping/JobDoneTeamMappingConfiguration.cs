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
            builder.HasKey(jdtm => new{jdtm.JobDoneId,jdtm.TeamId});

            // Configures the relationship between JobDone and JobDoneTeamMapping.
            builder
                .HasOne(jdtm => jdtm.JobDone) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction); 

            // Configures the relationship between Team and JobDoneTeamMapping.
            builder
                .HasOne(jdtm => jdtm.Team) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
