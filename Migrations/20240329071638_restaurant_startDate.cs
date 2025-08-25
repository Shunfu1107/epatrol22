using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class restaurant_startDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<DateTime>(
                name: "startDate",
                table: "restaurant",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taginfo");

            migrationBuilder.DropColumn(
                name: "restaurant_id",
                table: "tablecoordinate");

            migrationBuilder.DropColumn(
                name: "startDate",
                table: "restaurant");

            migrationBuilder.AddColumn<int>(
                name: "total_anchor",
                table: "restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_robot",
                table: "restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_tag",
                table: "restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "restaurant_id",
                table: "order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "restaurant_id",
                table: "additionalrequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
