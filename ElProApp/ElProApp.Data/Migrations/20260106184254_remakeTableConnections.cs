using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class remakeTableConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Material_Buildings_BuildingId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_BuildingId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Material");

            migrationBuilder.CreateTable(
                name: "BuildingMaterialMappings",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Foreign key referencing the Building entity.", collation: "ascii_general_ci"),
                    MaterialId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Foreign key referencing the Material entity.", collation: "ascii_general_ci"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "Quantity of material used at the Building."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    BuildingId1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MaterialId1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingMaterialMappings", x => new { x.BuildingId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_BuildingMaterialMappings_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildingMaterialMappings_Buildings_BuildingId1",
                        column: x => x.BuildingId1,
                        principalTable: "Buildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildingMaterialMappings_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildingMaterialMappings_Material_MaterialId1",
                        column: x => x.MaterialId1,
                        principalTable: "Material",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingMaterialMappings_BuildingId1",
                table: "BuildingMaterialMappings",
                column: "BuildingId1");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingMaterialMappings_MaterialId",
                table: "BuildingMaterialMappings",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingMaterialMappings_MaterialId1",
                table: "BuildingMaterialMappings",
                column: "MaterialId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingMaterialMappings");

            migrationBuilder.AddColumn<Guid>(
                name: "BuildingId",
                table: "Material",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Material",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                comment: "The quantity of the material.");

            migrationBuilder.CreateIndex(
                name: "IX_Material_BuildingId",
                table: "Material",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Material_Buildings_BuildingId",
                table: "Material",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
