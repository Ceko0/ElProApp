namespace ElProApp.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models;
    using static Common.EntityValidationConstants.Team;

    /// <summary>
    /// Configuration class for the Team entity, defining the schema for the Teams table.
    /// </summary>
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        /// <summary>
        /// Configures the Team entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the Team entity.</param>
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            // Sets the primary key for the Team entity.
            builder
                .HasKey(et => et.Id);

            // Configures the Name property with required validation and maximum length.
            builder
                .Property(et => et.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);
        }
    }
}
