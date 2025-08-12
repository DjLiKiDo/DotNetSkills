using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetSkills.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The team name"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Optional team description"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "SQL Server rowversion for optimistic concurrency"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.CheckConstraint("CK_Teams_Name_MinLength", "LEN([Name]) >= 2");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The user's full name"),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false, comment: "The user's email address"),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The user's role in the system (Admin, ProjectManager, Developer, Viewer)"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The user's current status (Active, Inactive, Suspended, Pending)"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "SQL Server rowversion for optimistic concurrency"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "The display name of the project"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Optional detailed description of the project"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Current status of the project (Active, Completed, OnHold, Cancelled, Planning)"),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the team responsible for this project"),
                    StartDate = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The actual start date of project work"),
                    EndDate = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The actual completion date of the project"),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The originally planned completion date"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "SQL Server rowversion for optimistic concurrency"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "dbo",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Stores project information and metadata for the project management system");

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the user who is a member of the team"),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the team the user belongs to"),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The role of the user within the team (Developer, ProjectManager, TeamLead, Viewer)"),
                    JoinedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "The date and time when the user joined the team"),
                    TeamId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "dbo",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId1",
                        column: x => x.TeamId1,
                        principalSchema: "dbo",
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Users_UserId1",
                        column: x => x.UserId1,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "The title/name of the task"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Detailed description of the task requirements and goals"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Current status of the task (ToDo, InProgress, InReview, Done, Cancelled)"),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Priority level of the task (Low, Medium, High, Critical)"),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The ID of the project this task belongs to"),
                    AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "The ID of the user assigned to this task (null if unassigned)"),
                    ParentTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "The ID of the parent task (null for top-level tasks)"),
                    EstimatedHours = table.Column<int>(type: "int", nullable: true, comment: "Estimated effort in hours to complete the task"),
                    ActualHours = table.Column<int>(type: "int", nullable: true, comment: "Actual effort in hours spent on the task (set when completed)"),
                    DueDate = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The target completion date for the task"),
                    StartedAt = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The date and time when work on the task began"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2(7)", precision: 7, nullable: true, comment: "The date and time when the task was completed"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "SQL Server rowversion for optimistic concurrency"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "dbo",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Tasks_Parent",
                        column: x => x.ParentTaskId,
                        principalSchema: "dbo",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssignedUser",
                        column: x => x.AssignedUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                },
                comment: "Stores task information with hierarchy support for project management");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedAt",
                schema: "dbo",
                table: "Projects",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name_Unique",
                schema: "dbo",
                table: "Projects",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PlannedEndDate",
                schema: "dbo",
                table: "Projects",
                column: "PlannedEndDate",
                filter: "[PlannedEndDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status_PlannedEndDate",
                schema: "dbo",
                table: "Projects",
                columns: new[] { "Status", "PlannedEndDate" },
                filter: "[Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status_TeamId",
                schema: "dbo",
                table: "Projects",
                columns: new[] { "Status", "TeamId" },
                filter: "[Status] IS NOT NULL AND [TeamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TeamId",
                schema: "dbo",
                table: "Projects",
                column: "TeamId",
                filter: "[TeamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreatedAt",
                schema: "dbo",
                table: "Tasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedUserId",
                schema: "dbo",
                table: "Tasks",
                column: "AssignedUserId",
                filter: "[AssignedUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedUserId_Status",
                schema: "dbo",
                table: "Tasks",
                columns: new[] { "AssignedUserId", "Status" },
                filter: "[AssignedUserId] IS NOT NULL AND [Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DueDate",
                schema: "dbo",
                table: "Tasks",
                column: "DueDate",
                filter: "[DueDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentTaskId",
                schema: "dbo",
                table: "Tasks",
                column: "ParentTaskId",
                filter: "[ParentTaskId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                schema: "dbo",
                table: "Tasks",
                column: "ProjectId",
                filter: "[ProjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId_Status",
                schema: "dbo",
                table: "Tasks",
                columns: new[] { "ProjectId", "Status" },
                filter: "[ProjectId] IS NOT NULL AND [Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Status_DueDate",
                schema: "dbo",
                table: "Tasks",
                columns: new[] { "Status", "DueDate" },
                filter: "[Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Status_Priority",
                schema: "dbo",
                table: "Tasks",
                columns: new[] { "Status", "Priority" },
                filter: "[Status] IS NOT NULL AND [Priority] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Title",
                schema: "dbo",
                table: "Tasks",
                column: "Title",
                filter: "[Title] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_JoinedAt",
                schema: "dbo",
                table: "TeamMembers",
                column: "JoinedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_Role",
                schema: "dbo",
                table: "TeamMembers",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId",
                schema: "dbo",
                table: "TeamMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_Role",
                schema: "dbo",
                table: "TeamMembers",
                columns: new[] { "TeamId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "TeamId1");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                schema: "dbo",
                table: "TeamMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId_TeamId_Unique",
                schema: "dbo",
                table: "TeamMembers",
                columns: new[] { "UserId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CreatedAt",
                schema: "dbo",
                table: "Teams",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name_Unique",
                schema: "dbo",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedAt",
                schema: "dbo",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Unique",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                schema: "dbo",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                schema: "dbo",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_Status",
                schema: "dbo",
                table: "Users",
                columns: new[] { "Role", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                schema: "dbo",
                table: "Users",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TeamMembers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "dbo");
        }
    }
}
