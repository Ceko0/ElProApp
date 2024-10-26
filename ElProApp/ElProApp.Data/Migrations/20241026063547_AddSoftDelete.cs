using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_buildingTeamMappings_buildings_BuildingId",
                table: "buildingTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_buildingTeamMappings_teams_TeamId",
                table: "buildingTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_AspNetUsers_UserId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_employeeTeamMappings_employees_EmployeeId",
                table: "employeeTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_employeeTeamMappings_teams_TeamId",
                table: "employeeTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_jobDoneTeamMappings_jobsDone_JobDoneId",
                table: "jobDoneTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_jobDoneTeamMappings_teams_TeamId",
                table: "jobDoneTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_jobsDone_jobs_JobId",
                table: "jobsDone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teams",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobsDone",
                table: "jobsDone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobs",
                table: "jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_jobDoneTeamMappings",
                table: "jobDoneTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employeeTeamMappings",
                table: "employeeTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_buildingTeamMappings",
                table: "buildingTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_buildings",
                table: "buildings");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "employees");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "Teams");

            migrationBuilder.RenameTable(
                name: "jobsDone",
                newName: "JobsDone");

            migrationBuilder.RenameTable(
                name: "jobs",
                newName: "Jobs");

            migrationBuilder.RenameTable(
                name: "jobDoneTeamMappings",
                newName: "JobDoneTeamMappings");

            migrationBuilder.RenameTable(
                name: "employeeTeamMappings",
                newName: "EmployeeTeamMappings");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "buildingTeamMappings",
                newName: "BuildingTeamMappings");

            migrationBuilder.RenameTable(
                name: "buildings",
                newName: "Buildings");

            migrationBuilder.RenameIndex(
                name: "IX_jobsDone_JobId",
                table: "JobsDone",
                newName: "IX_JobsDone_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_jobDoneTeamMappings_TeamId",
                table: "JobDoneTeamMappings",
                newName: "IX_JobDoneTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_jobDoneTeamMappings_JobDoneId",
                table: "JobDoneTeamMappings",
                newName: "IX_JobDoneTeamMappings_JobDoneId");

            migrationBuilder.RenameIndex(
                name: "IX_employeeTeamMappings_TeamId",
                table: "EmployeeTeamMappings",
                newName: "IX_EmployeeTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_UserId",
                table: "Employees",
                newName: "IX_Employees_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_buildingTeamMappings_TeamId",
                table: "BuildingTeamMappings",
                newName: "IX_BuildingTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_buildingTeamMappings_BuildingId",
                table: "BuildingTeamMappings",
                newName: "IX_BuildingTeamMappings_BuildingId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "The name of the team, limited by maximum length.",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "The name of the team with a maximum length");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Primary key and unique identifier for the team.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Primary key and unique identifier for the team");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Teams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the team is active or soft deleted.");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "JobsDone",
                type: "decimal(6,2)",
                nullable: false,
                comment: "Quantity of work completed.",
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldComment: "Quantity of work completed");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "JobsDone",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key for the job being done.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key for the job being done");

            migrationBuilder.AlterColumn<int>(
                name: "DaysForJob",
                table: "JobsDone",
                type: "int",
                nullable: false,
                comment: "Number of days spent completing the job.",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of days spent completing the job");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "JobsDone",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the job done record.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the job done record");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the job.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "JobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the Team entity.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobDoneId",
                table: "JobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the JobDone entity.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "JobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the mapping between JobDone and Team.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "EmployeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the Team entity.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key for the team.");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "EmployeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the Employee entity.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key for the employee.");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                comment: "Foreign key representing the user account associated with this employee.",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the employee is active or soft deleted.");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "BuildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the Team entity.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BuildingId",
                table: "BuildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key referencing the Building entity.",
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Buildings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the building is active or soft deleted.");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobsDone",
                table: "JobsDone",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTeamMappings",
                table: "EmployeeTeamMappings",
                columns: new[] { "EmployeeId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildingTeamMappings_Buildings_BuildingId",
                table: "BuildingTeamMappings",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildingTeamMappings_Teams_TeamId",
                table: "BuildingTeamMappings",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTeamMappings_Employees_EmployeeId",
                table: "EmployeeTeamMappings",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTeamMappings_Teams_TeamId",
                table: "EmployeeTeamMappings",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobDoneTeamMappings_JobsDone_JobDoneId",
                table: "JobDoneTeamMappings",
                column: "JobDoneId",
                principalTable: "JobsDone",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobDoneTeamMappings_Teams_TeamId",
                table: "JobDoneTeamMappings",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobsDone_Jobs_JobId",
                table: "JobsDone",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildingTeamMappings_Buildings_BuildingId",
                table: "BuildingTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_BuildingTeamMappings_Teams_TeamId",
                table: "BuildingTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTeamMappings_Employees_EmployeeId",
                table: "EmployeeTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTeamMappings_Teams_TeamId",
                table: "EmployeeTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDoneTeamMappings_JobsDone_JobDoneId",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDoneTeamMappings_Teams_TeamId",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_JobsDone_Jobs_JobId",
                table: "JobsDone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobsDone",
                table: "JobsDone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDoneTeamMappings",
                table: "JobDoneTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTeamMappings",
                table: "EmployeeTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingTeamMappings",
                table: "BuildingTeamMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Buildings");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "teams");

            migrationBuilder.RenameTable(
                name: "JobsDone",
                newName: "jobsDone");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "jobs");

            migrationBuilder.RenameTable(
                name: "JobDoneTeamMappings",
                newName: "jobDoneTeamMappings");

            migrationBuilder.RenameTable(
                name: "EmployeeTeamMappings",
                newName: "employeeTeamMappings");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "BuildingTeamMappings",
                newName: "buildingTeamMappings");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "buildings");

            migrationBuilder.RenameIndex(
                name: "IX_JobsDone_JobId",
                table: "jobsDone",
                newName: "IX_jobsDone_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDoneTeamMappings_TeamId",
                table: "jobDoneTeamMappings",
                newName: "IX_jobDoneTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDoneTeamMappings_JobDoneId",
                table: "jobDoneTeamMappings",
                newName: "IX_jobDoneTeamMappings_JobDoneId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeTeamMappings_TeamId",
                table: "employeeTeamMappings",
                newName: "IX_employeeTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_UserId",
                table: "employees",
                newName: "IX_employees_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BuildingTeamMappings_TeamId",
                table: "buildingTeamMappings",
                newName: "IX_buildingTeamMappings_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_BuildingTeamMappings_BuildingId",
                table: "buildingTeamMappings",
                newName: "IX_buildingTeamMappings_BuildingId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "teams",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "The name of the team with a maximum length",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "The name of the team, limited by maximum length.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "teams",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Primary key and unique identifier for the team",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Primary key and unique identifier for the team.");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "jobsDone",
                type: "decimal(6,2)",
                nullable: false,
                comment: "Quantity of work completed",
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldComment: "Quantity of work completed.");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "jobsDone",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key for the job being done",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key for the job being done.");

            migrationBuilder.AlterColumn<int>(
                name: "DaysForJob",
                table: "jobsDone",
                type: "int",
                nullable: false,
                comment: "Number of days spent completing the job",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of days spent completing the job.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "jobsDone",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the job done record",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the job done record.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "jobs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the job.");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "jobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the Team entity.");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobDoneId",
                table: "jobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the JobDone entity.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "jobDoneTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the mapping between JobDone and Team.");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "employeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key for the team.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the Team entity.");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "employeeTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Foreign key for the employee.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the Employee entity.");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "Foreign key representing the user account associated with this employee.");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "employees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key representing the team to which the employee belongs.");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeamId",
                table: "buildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the Team entity.");

            migrationBuilder.AlterColumn<Guid>(
                name: "BuildingId",
                table: "buildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the Building entity.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "buildingTeamMappings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the mapping record.");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teams",
                table: "teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobsDone",
                table: "jobsDone",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobs",
                table: "jobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_jobDoneTeamMappings",
                table: "jobDoneTeamMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employeeTeamMappings",
                table: "employeeTeamMappings",
                columns: new[] { "EmployeeId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_buildingTeamMappings",
                table: "buildingTeamMappings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_buildings",
                table: "buildings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_buildingTeamMappings_buildings_BuildingId",
                table: "buildingTeamMappings",
                column: "BuildingId",
                principalTable: "buildings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_buildingTeamMappings_teams_TeamId",
                table: "buildingTeamMappings",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_AspNetUsers_UserId",
                table: "employees",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employeeTeamMappings_employees_EmployeeId",
                table: "employeeTeamMappings",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employeeTeamMappings_teams_TeamId",
                table: "employeeTeamMappings",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_jobDoneTeamMappings_jobsDone_JobDoneId",
                table: "jobDoneTeamMappings",
                column: "JobDoneId",
                principalTable: "jobsDone",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_jobDoneTeamMappings_teams_TeamId",
                table: "jobDoneTeamMappings",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_jobsDone_jobs_JobId",
                table: "jobsDone",
                column: "JobId",
                principalTable: "jobs",
                principalColumn: "Id");
        }
    }
}
