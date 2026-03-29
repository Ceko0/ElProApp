namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using ElProApp.Data.Models.Mappings;

    /// <summary>
    /// Configures the <see cref="JobMaterialMapping"/> entity (legacy).
    /// </summary>
    public class JobMaterialMappingConfiguration
        : IEntityTypeConfiguration<JobMaterialMapping>
    {
        /// <summary>
        /// Configures the entity relationships and keys.
        /// </summary>
        /// <param name="builder">The entity builder.</param>
        public void Configure(EntityTypeBuilder<JobMaterialMapping> builder)
        {
            builder
                .HasKey(x => new { x.JobId, x.MaterialId });

            builder
                .HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.Material)
                .WithMany()
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}