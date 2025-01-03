namespace ElProApp.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    /// <summary>
    /// Configuration class for the JobDone entity, defining the schema for the JobDone table.
    /// </summary>
    public class JobDoneConfiguration : IEntityTypeConfiguration<JobDone>
    {
        /// <summary>
        /// Configures the JobDone entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the JobDone entity.</param>
        public void Configure(EntityTypeBuilder<JobDone> builder)
        {
            // Sets the primary key for the JobDone entity.
            builder
                .HasKey(jd => jd.Id);
            
            // Configures the DaysForJob property as required.
            builder
                .Property(jd => jd.DaysForJob)
                .IsRequired();

        }
    }
}
