namespace ElProApp.Data.Configurations.Mapping
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Models.Mappings;

    internal class JobDoneJobMappingConfiguration : IEntityTypeConfiguration<JobDoneJobMapping>
    {
        public void Configure(EntityTypeBuilder<JobDoneJobMapping> builder)
        {
            builder
                .HasKey(jdj => new { jdj.JobDoneId, jdj.JobId});

            builder
                .HasOne(jdj => jdj.JobDone)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(jdj => jdj.Job)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(jdj => jdj.Quantity)
                .HasColumnType("decimal(6, 2)");
        }
    }
}
