namespace ElProApp.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;
    using static Common.EntityValidationConstants.Building;

    /// <summary>
    /// Configuration class for the Building entity, defining the schema for the Building table.
    /// </summary>
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        /// <summary>
        /// Configures the Building entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the Building entity.</param>
        public void Configure(EntityTypeBuilder<Building> builder)
        {
            // Sets the primary key for the Building entity.
            builder.HasKey(b => b.Id);

            // Configures the Name property as required with a maximum length.
            builder.Property(b => b.Name)
                .IsRequired() // Name must not be null
                .HasMaxLength(BuildingNameMaxLength); // Maximum length defined by BuildingNameMaxLength constant

            // Configures the Location property as required with a maximum length.
            builder.Property(b => b.Location)
                .IsRequired() // Location must not be null
                .HasMaxLength(LocationMaxLength); // Maximum length defined by LocationMaxLength constant
        }
    }
}
