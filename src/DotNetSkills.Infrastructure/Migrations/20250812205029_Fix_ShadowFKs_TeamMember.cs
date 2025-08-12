using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetSkills.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ShadowFKs_TeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId1",
                schema: "dbo",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_UserId1",
                schema: "dbo",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId1",
                schema: "dbo",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_UserId1",
                schema: "dbo",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                schema: "dbo",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "dbo",
                table: "TeamMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeamId1",
                schema: "dbo",
                table: "TeamMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                schema: "dbo",
                table: "TeamMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "TeamId1");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "TeamId1",
                principalSchema: "dbo",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_UserId1",
                schema: "dbo",
                table: "TeamMembers",
                column: "UserId1",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
