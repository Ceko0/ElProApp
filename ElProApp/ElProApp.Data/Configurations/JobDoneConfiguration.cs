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

            // Configures the Quantity property as required with a specific decimal type.
            builder
                .Property(jd => jd.Quantity)
                .IsRequired()
                .HasColumnType("decimal(6, 2)"); // 6 digits total, 2 after the decimal point

            // Configures the DaysForJob property as required.
            builder
                .Property(jd => jd.DaysForJob)
                .IsRequired();

            // Configures the one-to-many relationship between JobDone and Job.
            builder
                .HasOne(jd => jd.Job) // Each JobDone is associated with one Job
                .WithMany(j => j.JobsDone) // A Job can have multiple JobDone records
                .HasForeignKey(jd => jd.JobId) // Foreign key property in JobDone
                .OnDelete(DeleteBehavior.NoAction); // Prevents deletion of Job if JobDone records exist
        }
    }
}
