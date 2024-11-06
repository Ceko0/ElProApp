using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixingKeyFOrMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropIndex(
                name: "IX_JobDoneTeamMappings_JobDoneId",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings");

            migrationBuilder.DropIndex(
                name: "IX_BuildingTeamMappings_BuildingId",
                table: "BuildingTeamMappings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings",
                columns: new[] { "JobDoneId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings",
                columns: new[] { "BuildingId", "TeamId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobDoneTeamMappings_JobDoneId",
                table: "JobDoneTeamMappings",
                column: "JobDoneId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingTeamMappings_BuildingId",
                table: "BuildingTeamMappings",
                column: "BuildingId");
        }
    }
}
