using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixinfMappingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "JobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the mapping between JobDone and Team.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "EmployeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the mapping between Employee and Team.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BuildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the mapping record.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "JobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the mapping between JobDone and Team.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "EmployeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the mapping between Employee and Team.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BuildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the mapping record.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
