namespace ElProApp.Data.Configurations.Mapping
{
    using Models.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Configuration class for the BuildingTeamMapping entity,
    /// defining the schema for the BuildingTeamMapping table.
    /// </summary>
    public class BuildingTeamMappingConfiguration : IEntityTypeConfiguration<BuildingTeamMapping>
    {
        /// <summary>
        /// Configures the BuildingTeamMapping entity properties and relationships.
        /// </summary>
        /// <param name="builder">EntityTypeBuilder used to configure the BuildingTeamMapping entity.</param>
        public void Configure(EntityTypeBuilder<BuildingTeamMapping> builder)
        {
            // Sets the primary key for the BuildingTeamMapping entity.
            builder.HasKey(btm => new{ btm.BuildingId, btm.TeamId});

            // Configures the relationship between Building and BuildingTeamMapping.
            builder
                .HasOne(btm => btm.Building) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction);

            // Configures the relationship between Team and BuildingTeamMapping.
            builder
                .HasOne(btm => btm.Team) 
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
