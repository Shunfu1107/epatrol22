using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class update_additionalrequestdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_additionalrequestdetail_additionalrequest_additionalrequest_id",
                table: "additionalrequestdetail");

            migrationBuilder.RenameColumn(
                name: "additionalrequest_id",
                table: "additionalrequestdetail",
                newName: "additionalrequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "additionalrequest",
                table: "additionalrequestdetail",
                newName: "additionalrequest_id");

            migrationBuilder.CreateIndex(
                name: "IX_additionalrequestdetail_additionalrequest_id",
                table: "additionalrequestdetail",
                column: "additionalrequest_id");

            migrationBuilder.AddForeignKey(
                name: "FK_additionalrequestdetail_additionalrequest_additionalrequest_id",
                table: "additionalrequestdetail",
                column: "additionalrequest_id",
                principalTable: "additionalrequest",
                principalColumn: "additionalrequest_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
