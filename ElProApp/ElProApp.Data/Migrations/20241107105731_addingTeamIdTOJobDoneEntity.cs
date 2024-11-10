using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingTeamIdTOJobDoneEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "JobsDone",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key for the team responsible for completing the job.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "JobsDone");
        }
    }
}
