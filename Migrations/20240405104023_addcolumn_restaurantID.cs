using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class addcolumn_restaurantID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "restaurant_id",
                table: "order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "branch_additionalrequest_id",
                table: "additionalrequest",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "restaurant_id",
                table: "order");

            migrationBuilder.DropColumn(
                name: "branch_additionalrequest_id",
                table: "additionalrequest");

            migrationBuilder.DropColumn(
                name: "restaurant_id",
                table: "additionalrequest");
        }
    }
}
