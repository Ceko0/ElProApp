namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class JobMaterialMappingConfiguration : IEntityTypeConfiguration<JobMaterialMapping>
    {
        public void Configure(EntityTypeBuilder<JobMaterialMapping> builder)
        {
            builder.HasKey(x => x.JobId);

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
