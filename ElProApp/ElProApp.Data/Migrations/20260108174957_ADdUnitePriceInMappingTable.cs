using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ADdUnitePriceInMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "JobDoneMaterialMappings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobDoneMaterialMappings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "JobDoneMaterialMappings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Snapshot of material unit price at the time of job execution.");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "BuildingMaterialMappings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "JobDoneMaterialMappings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobDoneMaterialMappings");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "JobDoneMaterialMappings");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "BuildingMaterialMappings");
        }
    }
}
