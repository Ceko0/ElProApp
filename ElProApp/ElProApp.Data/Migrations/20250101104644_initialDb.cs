using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElProApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier for the building."),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The name of the building with a minimum of 3 and a maximum of 50 characters."),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The location of the building with a minimum of 10 and a maximum of 100 characters."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the building is active or soft deleted."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    DeletedDate = table.Column<DateTime>(type: "date", nullable: true, comment: "The date when the record was deleted (logically deleted).")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier for the job."),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The name of the job with a maximum of 50 characters."),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false, comment: "The price of the job with up to 4 digits before the decimal point and up to 2 digits after."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the job is active or soft deleted."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    DeletedDate = table.Column<DateTime>(type: "date", nullable: true, comment: "The date when the record was deleted (logically deleted).")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary key and unique identifier for the team."),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The name of the team, limited by maximum length."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the team is active or soft deleted."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    DeletedDate = table.Column<DateTime>(type: "date", nullable: true, comment: "The date when the record was deleted (logically deleted).")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier for the employee."),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The first name of the employee with a maximum of 20 characters."),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The last name of the employee with a maximum of 20 characters."),
                    Wages = table.Column<decimal>(type: "decimal(6,2)", nullable: false, comment: "The wages of the employee with up to 6 digits before the decimal point and up to 2 digits after."),
                    MoneyToTake = table.Column<decimal>(type: "decimal(18,2)", nullable: false, comment: "The money the employee has to take, must be a positive value."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the employee is active or soft deleted."),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "Foreign key representing the user account associated with this employee."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    DeletedDate = table.Column<DateTime>(type: "date", nullable: true, comment: "The date when the record was deleted (logically deleted).")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobsDone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier for the job done record."),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The name of the job with a maximum of 50 characters."),
                    DaysForJob = table.Column<int>(type: "int", nullable: false, comment: "Number of days spent completing the job."),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key for the building where was completing the job."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the jobDone is active or soft deleted."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created."),
                    DeletedDate = table.Column<DateTime>(type: "date", nullable: true, comment: "The date when the record was deleted (logically deleted).")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsDone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobsDone_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingTeamMappings",
                columns: table => new
                {
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Building entity."),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Team entity."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingTeamMappings", x => new { x.BuildingId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_BuildingTeamMappings_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BuildingTeamMappings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTeamMappings",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Employee entity."),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Team entity."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTeamMappings", x => new { x.EmployeeId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_EmployeeTeamMappings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeTeamMappings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobDoneJobMapping",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobDoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(6,2)", nullable: false, comment: "The quantity of the job with up to 6 digits before the decimal point and up to 2 digits after."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDoneJobMapping", x => new { x.JobDoneId, x.JobId });
                    table.ForeignKey(
                        name: "FK_JobDoneJobMapping_JobsDone_JobDoneId",
                        column: x => x.JobDoneId,
                        principalTable: "JobsDone",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobDoneJobMapping_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobDoneTeamMappings",
                columns: table => new
                {
                    JobDoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the JobDone entity."),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Team entity."),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false, comment: "The date when the record was created.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDoneTeamMappings", x => new { x.JobDoneId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_JobDoneTeamMappings_JobsDone_JobDoneId",
                        column: x => x.JobDoneId,
                        principalTable: "JobsDone",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobDoneTeamMappings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingTeamMappings_TeamId",
                table: "BuildingTeamMappings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeamMappings_TeamId",
                table: "EmployeeTeamMappings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDoneJobMapping_JobId",
                table: "JobDoneJobMapping",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDoneTeamMappings_TeamId",
                table: "JobDoneTeamMappings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsDone_BuildingId",
                table: "JobsDone",
                column: "BuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BuildingTeamMappings");

            migrationBuilder.DropTable(
                name: "EmployeeTeamMappings");

            migrationBuilder.DropTable(
                name: "JobDoneJobMapping");

            migrationBuilder.DropTable(
                name: "JobDoneTeamMappings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "JobsDone");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Buildings");
        }
    }
}
