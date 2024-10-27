using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixinWageRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Wages",
                table: "Employees",
                type: "decimal(6,2)",
                nullable: false,
                comment: "The wages of the employee with up to 6 digits before the decimal point and up to 2 digits after.",
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)",
                oldComment: "The wages of the employee with up to 4 digits before the decimal point and up to 2 digits after.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Wages",
                table: "Employees",
                type: "decimal(4,2)",
                nullable: false,
                comment: "The wages of the employee with up to 4 digits before the decimal point and up to 2 digits after.",
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldComment: "The wages of the employee with up to 6 digits before the decimal point and up to 2 digits after.");
        }
    }
}
