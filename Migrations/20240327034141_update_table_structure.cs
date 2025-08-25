using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class update_table_structure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "additionalrequestdetail");

            migrationBuilder.DropTable(
                name: "anchor");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "restaurant");

            migrationBuilder.DropTable(
                name: "robot");

            migrationBuilder.DropTable(
                name: "tablecoordinate");

            migrationBuilder.DropTable(
                name: "taginfo");

            migrationBuilder.DropTable(
                name: "additionalrequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

        }
    }
}
