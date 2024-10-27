namespace ElProApp.Data.Configurations.Mapping
{
    using ElProApp.Data.Models.Mappings;
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
            builder.HasKey(btm => btm.Id);

            // Configures the relationship between Building and BuildingTeamMapping.
            builder
                .HasOne(btm => btm.Building) // Each BuildingTeamMapping has one Building
                .WithMany(b => b.TeamsOnBuilding) // A Building can have many BuildingTeamMappings
                .HasForeignKey(btm => btm.BuildingId) // Foreign key defined in BuildingTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship

            // Configures the relationship between Team and BuildingTeamMapping.
            builder
                .HasOne(btm => btm.Team) // Each BuildingTeamMapping has one Team
                .WithMany(t => t.BuildingWithTeam) // A Team can have many BuildingTeamMappings
                .HasForeignKey(btm => btm.TeamId) // Foreign key defined in BuildingTeamMapping
                .OnDelete(DeleteBehavior.NoAction); // Specify delete behavior for this relationship
        }
    }
}
