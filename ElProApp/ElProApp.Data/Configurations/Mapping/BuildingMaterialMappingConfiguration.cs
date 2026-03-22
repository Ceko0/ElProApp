namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using global::ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configuration for the <see cref="BuildingMaterialMapping"/> entity.
    /// Defines the relationships between Building and Material and prevents EF Core
    /// from creating shadow foreign keys such as BuildingId1 and MaterialId1.
    /// </summary>
    public class BuildingMaterialMappingConfiguration : IEntityTypeConfiguration<BuildingMaterialMapping>
    {
        /// <summary>
        /// Configures the entity properties and relationships.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity.</param>
        public void Configure(EntityTypeBuilder<BuildingMaterialMapping> builder)
        {
            builder
                .HasKey(x => new { x.BuildingId, x.MaterialId });

            builder
                .HasOne(x => x.Building)
                .WithMany(b => b.Materials)
                .HasForeignKey(x => x.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Material)
                .WithMany(m => m.Buildings)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}