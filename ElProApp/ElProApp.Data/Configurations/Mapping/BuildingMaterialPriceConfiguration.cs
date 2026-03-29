namespace ElProApp.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configuration for <see cref="BuildingMaterialPrice"/>.
    /// </summary>
    public class BuildingMaterialPriceConfiguration
        : IEntityTypeConfiguration<BuildingMaterialPrice>
    {
        /// <summary>
        /// Configures the entity.
        /// </summary>
        public void Configure(EntityTypeBuilder<BuildingMaterialPrice> builder)
        {
            builder.HasKey(x => new { x.BuildingId, x.MaterialId });

            builder
                .HasOne(x => x.Building)
                .WithMany()
                .HasForeignKey(x => x.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}