using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class addcolumn_branchorderid_ordertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branch_order_id",
                table: "order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "branch_order_id",
                table: "order");
        }
    }
}
