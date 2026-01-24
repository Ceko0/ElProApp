namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configuration class for the BuildingMaterialMapping entity,
    /// defining the schema for the BuildingMaterialMapping table.
    /// </summary>
    public class BuildingMaterialMappingConfiguration : IEntityTypeConfiguration<BuildingMaterialMapping>
    {
        /// <summary>
        /// Configures the BuildingMaterialMapping entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the BuildingMaterialMapping entity.</param>
        public void Configure(EntityTypeBuilder<BuildingMaterialMapping> builder)
        {
            // Sets the primary key for the BuildingMaterialMapping entity.
            builder.HasKey(bmm => new { bmm.BuildingId, bmm.MaterialId });

            // Configures the relationship between Building and BuildingMaterialMapping.
            builder
                .HasOne(bmm => bmm.Building)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            // Configures the relationship between Material and BuidlingMaterialMapping.
            builder
                .HasOne(bmm => bmm.Material)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
