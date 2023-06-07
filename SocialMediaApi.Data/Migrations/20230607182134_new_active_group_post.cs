using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaApi.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class new_active_group_post : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "GroupPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpireDate",
                table: "GroupPosts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "NewGroupPost_Rank",
                table: "GroupPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "GroupPosts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "GroupPosts");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "GroupPosts");

            migrationBuilder.DropColumn(
                name: "NewGroupPost_Rank",
                table: "GroupPosts");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "GroupPosts");
        }
    }
}
