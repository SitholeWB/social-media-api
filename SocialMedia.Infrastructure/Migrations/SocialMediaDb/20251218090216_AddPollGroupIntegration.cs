using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Infrastructure.Migrations.SocialMediaDb
{
    /// <inheritdoc />
    public partial class AddPollGroupIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Polls",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Polls_GroupId",
                table: "Polls",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_Groups_GroupId",
                table: "Polls",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_Groups_GroupId",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_GroupId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Polls");
        }
    }
}
