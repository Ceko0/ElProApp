namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configures the <see cref="JobDoneMaterialMapping"/> entity.
    /// </summary>
    public class JobDoneMaterialMappingConfiguration
        : IEntityTypeConfiguration<JobDoneMaterialMapping>
    {
        /// <summary>
        /// Configures the entity relationships and properties.
        /// </summary>
        /// <param name="builder">The entity builder.</param>
        public void Configure(EntityTypeBuilder<JobDoneMaterialMapping> builder)
        {
            builder
                .HasKey(x => new { x.JobDoneId, x.MaterialId });

            builder
                .HasOne(x => x.JobDone)
                .WithMany(jd => jd.Materials)
                .HasForeignKey(x => x.JobDoneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Material)
                .WithMany(m => m.JobDones)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(x => x.Quantity)
                .HasPrecision(18, 2);

            builder
                .Property(x => x.UnitPrice)
                .HasPrecision(18, 2);

            builder
                .Property(x => x.CreatedDate)
                .HasColumnType("date");
        }
    }
}