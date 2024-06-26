using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tour_planner.Migrations
{
    /// <inheritdoc />
    public partial class AdjustSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing tables if they exist
            migrationBuilder.DropTable(
                name: "TourLogs"
            );

            migrationBuilder.DropTable(
                name: "Tours"
            );

            // Create Tours table
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    TransportType = table.Column<string>(type: "text", nullable: false),
                    TourDistance = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RouteImage = table.Column<string>(type: "text", nullable: false),
                    Popularity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ChildFriendliness = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.TourId);
                });

            // Create TourLogs table
            migrationBuilder.CreateTable(
                name: "TourLogs",
                columns: table => new
                {
                    TourLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    StatusMessage = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    TotalDistance = table.Column<double>(type: "double precision", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    TourId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLogs", x => x.TourLogId);
                    table.ForeignKey(
                        name: "FK_TourLogs_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourLogs_TourId",
                table: "TourLogs",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourLogs");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
