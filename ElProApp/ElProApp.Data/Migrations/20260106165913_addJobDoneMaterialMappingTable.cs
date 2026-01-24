using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addJobDoneMaterialMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobDoneMaterialMappings",
                columns: table => new
                {
                    JobDoneId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Foreign key referencing the JobDone entity.", collation: "ascii_general_ci"),
                    MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Foreign key referencing the Material entity.", collation: "ascii_general_ci"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "Quantity of material used for the job."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDoneMaterialMappings", x => new { x.JobDoneId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_JobDoneMaterialMappings_JobsDone_JobDoneId",
                        column: x => x.JobDoneId,
                        principalTable: "JobsDone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobDoneMaterialMappings_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JobDoneMaterialMappings_MaterialId",
                table: "JobDoneMaterialMappings",
                column: "MaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobDoneMaterialMappings");
        }
    }
}
