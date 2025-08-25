using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class User_ReceivedEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceivedEmail",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "ReceivedEmail",
               table: "User");
        }
    }
}
