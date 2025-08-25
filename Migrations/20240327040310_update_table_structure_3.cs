using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class update_table_structure_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "additionalrequest",
               columns: table => new
               {
                   additionalrequest_id = table.Column<int>(type: "int", nullable: false)
              .Annotation("SqlServer:Identity", "1, 1"),
                   delivery_starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                   delivery_endtime = table.Column<DateTime>(type: "datetime2", nullable: false),
                   robot_id = table.Column<int>(type: "int", nullable: false)
               },
              constraints: table =>
              {
                  table.PrimaryKey("PK_additionalrequest", x => x.additionalrequest_id);
              });

            migrationBuilder.CreateTable(
                name: "anchor",
                columns: table => new
                {
                    anchor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ismain_anchor = table.Column<int>(type: "int", nullable: false),
                    X_Axis = table.Column<double>(type: "float", nullable: false),
                    Y_Axis = table.Column<double>(type: "float", nullable: false),
                    anchor_address = table.Column<string>(type: "varchar(50)", nullable: false),
                    restaurant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anchor", x => x.anchor_id);
                });


            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    table_no = table.Column<int>(type: "int", nullable: false),
                    delivery_starttime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delivery_endtime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    robot_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.order_id);
                });


            migrationBuilder.CreateTable(
                name: "restaurant",
                columns: table => new
                {
                    restaurant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    restaurant_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    restaurant_address = table.Column<string>(type: "varchar(200)", nullable: false),
                    manager = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurant", x => x.restaurant_id);
                });

            migrationBuilder.CreateTable(
                name: "robot",
                columns: table => new
                {
                    robot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    robot_serialnum = table.Column<string>(type: "varchar(100)", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    active = table.Column<int>(type: "int", nullable: false),
                    restaurant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot", x => x.robot_id);
                });

            migrationBuilder.CreateTable(
                name: "tablecoordinate",
                columns: table => new
                {
                    tablecoordinate_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    point_1x = table.Column<double>(type: "float", nullable: false),
                    point_1y = table.Column<double>(type: "float", nullable: false),
                    point_2x = table.Column<double>(type: "float", nullable: false),
                    point_2y = table.Column<double>(type: "float", nullable: false),
                    point_3x = table.Column<double>(type: "float", nullable: false),
                    point_3y = table.Column<double>(type: "float", nullable: false),
                    point_4x = table.Column<double>(type: "float", nullable: false),
                    point_4y = table.Column<double>(type: "float", nullable: false),
                    table_id = table.Column<int>(type: "int", nullable: false),
                    restaurant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tablecoordinate", x => x.tablecoordinate_id);
                });

            migrationBuilder.CreateTable(
                name: "taginfo",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag_address = table.Column<string>(type: "varchar(100)", nullable: false),
                    restaurant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taginfo", x => x.tag_id);
                });


            migrationBuilder.CreateTable(
                name: "additionalrequestdetail",
                columns: table => new
                {
                    requestdetail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    table_no = table.Column<int>(type: "int", nullable: false),
                    item_type = table.Column<int>(type: "int", nullable: false),
                    item_qty = table.Column<int>(type: "int", nullable: false),
                    additionalrequest_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_additionalrequestdetail", x => x.requestdetail_id);
                    table.ForeignKey(
                        name: "FK_additionalrequestdetail_additionalrequest_additionalrequest_id",
                        column: x => x.additionalrequest_id,
                        principalTable: "additionalrequest",
                        principalColumn: "additionalrequest_id",
                        onDelete: ReferentialAction.Cascade);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
