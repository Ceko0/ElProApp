namespace ElProApp.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using static Common.EntityValidationConstants.Job;
    using Models;

    /// <summary>
    /// Configuration class for the Job entity, defining the schema for the Job table.
    /// </summary>
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        /// <summary>
        /// Configures the Job entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the Job entity.</param>
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            // Sets the primary key for the Job entity.
            builder
                .HasKey(j => j.Id);

            // Configures the Name property as required with a maximum length.
            builder
                .Property(j => j.Name)
                .IsRequired() // Name must not be null
                .HasMaxLength(nameMaxLength); // Maximum length of the name

            // Configures the Price property as required with a specific decimal type.
            builder
                .Property(j => j.Price)
                .IsRequired() // Price must not be null
                .HasColumnType("decimal(6, 2)"); // 4 digits total, 2 after the decimal point
        }
    }
}
