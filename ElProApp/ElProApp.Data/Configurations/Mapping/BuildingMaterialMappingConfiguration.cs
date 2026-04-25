namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configures the <see cref="BuildingMaterialMapping"/> entity.
    /// </summary>
    public class BuildingMaterialMappingConfiguration
        : IEntityTypeConfiguration<BuildingMaterialMapping>
    {
        /// <summary>
        /// Configures the entity relationships and keys.
        /// </summary>
        /// <param name="builder">The entity builder.</param>
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