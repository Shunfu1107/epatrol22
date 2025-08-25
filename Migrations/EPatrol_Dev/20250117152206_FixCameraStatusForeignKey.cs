using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations.EPatrol_Dev
{
    /// <inheritdoc />
    public partial class FixCameraStatusForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIModel",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Detection = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    URL = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AIModel__E8D7A12C2F37B63D", x => x.ModelId);
                });

            migrationBuilder.CreateTable(
                name: "Camera",
                columns: table => new
                {
                    CameraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    URL = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Camera__F971E0C832636AFC", x => x.CameraId);
                });

            migrationBuilder.CreateTable(
                name: "CameraHealthResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraId = table.Column<int>(type: "int", nullable: false),
                    CameraName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraHealthResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guard",
                columns: table => new
                {
                    GuardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Guard__803F90F542A0A434", x => x.GuardId);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Level = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__E7FEA497D8C43CA8", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Location_Camera",
                columns: table => new
                {
                    Location_CameraID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RTSP_URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__657B0DE3168DABFC", x => x.Location_CameraID);
                });

            migrationBuilder.CreateTable(
                name: "PatrolType",
                columns: table => new
                {
                    PatrolTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PatrolTy__51B85F683DFA5205", x => x.PatrolTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Day = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__9C8A5B49304D40F9", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleCameraCheck",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Day = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__9C8A5B49A0E5237E", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "CheckList",
                columns: table => new
                {
                    CheckListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckListName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Model = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CheckLis__56318341C1EB7173", x => x.CheckListId);
                    table.ForeignKey(
                        name: "FK__CheckList__Model__73501C2F",
                        column: x => x.ModelId,
                        principalTable: "AIModel",
                        principalColumn: "ModelId");
                });

            migrationBuilder.CreateTable(
                name: "CheckPoint",
                columns: table => new
                {
                    CheckPointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckPointName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CheckPoi__185C6FE6B9AD55BD", x => x.CheckPointId);
                    table.ForeignKey(
                        name: "FK_CheckPointLocation",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateTable(
                name: "CameraStatus",
                columns: table => new
                {
                    CamareStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Location_CameraID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CameraId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CameraSt__DEF2F8CB81AF87AD", x => x.CamareStatusId);
                    table.ForeignKey(
                        name: "FK_CameraStatus_Camera_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Camera",
                        principalColumn: "CameraId");
                    table.ForeignKey(
                        name: "FK_CameraStatus_Location_Camera",
                        column: x => x.Location_CameraID,
                        principalTable: "Location_Camera",
                        principalColumn: "Location_CameraID");
                });

            migrationBuilder.CreateTable(
                name: "Camera_CameraScheduleCheck",
                columns: table => new
                {
                    CameraId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camera_CameraScheduleCheck", x => new { x.CameraId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_CameraScheduleCheck_Camera",
                        column: x => x.CameraId,
                        principalTable: "Camera",
                        principalColumn: "CameraId");
                    table.ForeignKey(
                        name: "FK_CameraScheduleCheck_ScheduleCameraCheck",
                        column: x => x.ScheduleId,
                        principalTable: "ScheduleCameraCheck",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateTable(
                name: "CheckPoint_Camera",
                columns: table => new
                {
                    CheckPointId = table.Column<int>(type: "int", nullable: false),
                    CameraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CheckPoi__87CB71EA0AB1B56D", x => new { x.CheckPointId, x.CameraId });
                    table.ForeignKey(
                        name: "FK__CheckPoin__Camer__038683F8",
                        column: x => x.CameraId,
                        principalTable: "Camera",
                        principalColumn: "CameraId");
                    table.ForeignKey(
                        name: "FK__CheckPoin__Check__02925FBF",
                        column: x => x.CheckPointId,
                        principalTable: "CheckPoint",
                        principalColumn: "CheckPointId");
                });

            migrationBuilder.CreateTable(
                name: "CheckPoint_CheckList",
                columns: table => new
                {
                    CheckPointId = table.Column<int>(type: "int", nullable: false),
                    CheckListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CheckPoi__1D3F77D2F54A12A8", x => new { x.CheckPointId, x.CheckListId });
                    table.ForeignKey(
                        name: "FK__CheckPoin__Check__0662F0A3",
                        column: x => x.CheckPointId,
                        principalTable: "CheckPoint",
                        principalColumn: "CheckPointId");
                    table.ForeignKey(
                        name: "FK__CheckPoin__Check__075714DC",
                        column: x => x.CheckListId,
                        principalTable: "CheckList",
                        principalColumn: "CheckListId");
                });

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CheckPointId = table.Column<int>(type: "int", nullable: true),
                    ScheduleId = table.Column<int>(type: "int", nullable: true),
                    PatrolTypeId = table.Column<int>(type: "int", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Route__80979B4DE556DF14", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK__Route__CheckPoin__7814D14C",
                        column: x => x.CheckPointId,
                        principalTable: "CheckPoint",
                        principalColumn: "CheckPointId");
                    table.ForeignKey(
                        name: "FK__Route__PatrolTyp__79FD19BE",
                        column: x => x.PatrolTypeId,
                        principalTable: "PatrolType",
                        principalColumn: "PatrolTypeId");
                    table.ForeignKey(
                        name: "FK__Route__ScheduleI__7908F585",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateTable(
                name: "Patrol",
                columns: table => new
                {
                    PatrolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RouteId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckListName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckPointName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patrol__15509D88E9BE0021", x => x.PatrolId);
                    table.ForeignKey(
                        name: "FK_PatrolGuard",
                        column: x => x.GuardId,
                        principalTable: "Guard",
                        principalColumn: "GuardId");
                    table.ForeignKey(
                        name: "FK__Patrol__RouteId__7CD98669",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "RouteId");
                });

            migrationBuilder.CreateTable(
                name: "Route_CheckPoint",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    CheckPointId = table.Column<int>(type: "int", nullable: false),
                    CameraId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Coordinate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Route_Ch__F1125DB3D024B649", x => new { x.RouteId, x.CheckPointId });
                    table.ForeignKey(
                        name: "FK__Route_Che__Check__0EF836A4",
                        column: x => x.CheckPointId,
                        principalTable: "CheckPoint",
                        principalColumn: "CheckPointId");
                    table.ForeignKey(
                        name: "FK__Route_Che__Route__0E04126B",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "RouteId");
                });

            migrationBuilder.CreateTable(
                name: "Route_Schedule",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Route_Sc__095F3EF93FB8A95C", x => new { x.RouteId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK__Route_Sch__Route__0A338187",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "RouteId");
                    table.ForeignKey(
                        name: "FK__Route_Sch__Sched__0B27A5C0",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    PatrolId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Report__D5BD48051DA89B60", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK__Report__PatrolId__7FB5F314",
                        column: x => x.PatrolId,
                        principalTable: "Patrol",
                        principalColumn: "PatrolId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Camera_CameraScheduleCheck_ScheduleId",
                table: "Camera_CameraScheduleCheck",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraStatus_CameraId",
                table: "CameraStatus",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraStatus_Location_CameraID",
                table: "CameraStatus",
                column: "Location_CameraID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckList_ModelId",
                table: "CheckList",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPoint_LocationId",
                table: "CheckPoint",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPoint_Camera_CameraId",
                table: "CheckPoint_Camera",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckPoint_CheckList_CheckListId",
                table: "CheckPoint_CheckList",
                column: "CheckListId");

            migrationBuilder.CreateIndex(
                name: "IX_Patrol_GuardId",
                table: "Patrol",
                column: "GuardId");

            migrationBuilder.CreateIndex(
                name: "IX_Patrol_RouteId",
                table: "Patrol",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_PatrolId",
                table: "Report",
                column: "PatrolId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_CheckPointId",
                table: "Route",
                column: "CheckPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_PatrolTypeId",
                table: "Route",
                column: "PatrolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_ScheduleId",
                table: "Route",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_CheckPoint_CheckPointId",
                table: "Route_CheckPoint",
                column: "CheckPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_Schedule_ScheduleId",
                table: "Route_Schedule",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Camera_CameraScheduleCheck");

            migrationBuilder.DropTable(
                name: "CameraHealthResult");

            migrationBuilder.DropTable(
                name: "CameraStatus");

            migrationBuilder.DropTable(
                name: "CheckPoint_Camera");

            migrationBuilder.DropTable(
                name: "CheckPoint_CheckList");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Route_CheckPoint");

            migrationBuilder.DropTable(
                name: "Route_Schedule");

            migrationBuilder.DropTable(
                name: "ScheduleCameraCheck");

            migrationBuilder.DropTable(
                name: "Location_Camera");

            migrationBuilder.DropTable(
                name: "Camera");

            migrationBuilder.DropTable(
                name: "CheckList");

            migrationBuilder.DropTable(
                name: "Patrol");

            migrationBuilder.DropTable(
                name: "AIModel");

            migrationBuilder.DropTable(
                name: "Guard");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "CheckPoint");

            migrationBuilder.DropTable(
                name: "PatrolType");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
