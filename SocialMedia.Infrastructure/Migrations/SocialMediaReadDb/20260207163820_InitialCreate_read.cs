using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Infrastructure.Migrations.SocialMediaReadDb
{
    /// <inheritdoc />
    public partial class InitialCreate_read : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorProfilePicUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AdminTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reactions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorProfilePicUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    StatusFullScreen = table.Column<bool>(type: "bit", nullable: false),
                    ReactionCount = table.Column<int>(type: "int", nullable: false),
                    CommentCount = table.Column<int>(type: "int", nullable: false),
                    TrendingScore = table.Column<double>(type: "float", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RankUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdminTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Media = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reactions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TopComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatsRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatsType = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalPosts = table.Column<int>(type: "int", nullable: false),
                    ActiveUsers = table.Column<int>(type: "int", nullable: false),
                    NewPosts = table.Column<int>(type: "int", nullable: false),
                    ResultingComments = table.Column<int>(type: "int", nullable: false),
                    ResultingReactions = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReactionBreakdown = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatsRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentReads_PostId",
                table: "CommentReads",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReads_GroupId",
                table: "PostReads",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommentCount",
                table: "PostReads",
                column: "CommentCount");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt",
                table: "PostReads",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_RankScore_CreatedAt",
                table: "PostReads",
                columns: new[] { "TrendingScore", "CreatedAt" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ReactionCount",
                table: "PostReads",
                column: "ReactionCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentReads");

            migrationBuilder.DropTable(
                name: "PostReads");

            migrationBuilder.DropTable(
                name: "StatsRecords");
        }
    }
}
