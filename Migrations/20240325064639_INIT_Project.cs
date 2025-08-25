using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPortalV8.Migrations
{
    /// <inheritdoc />
    public partial class INIT_Project : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityAccessLevel",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityAccessLevel", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityAccessLevelDetail",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityAccessLevelPvid = table.Column<long>(type: "bigint", nullable: false),
                    IdentityModulePvid = table.Column<long>(type: "bigint", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityAccessLevelDetail", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityAccessLevelExclusive",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserPvid = table.Column<long>(type: "bigint", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accessible = table.Column<bool>(type: "bit", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityAccessLevelExclusive", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityModule",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityModule", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityModuleOrganize",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityModulePvid = table.Column<long>(type: "bigint", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityModuleOrganize", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    PvId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    StartActiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndActiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.PvId);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleAccess",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityRolePvid = table.Column<long>(type: "bigint", nullable: false),
                    IdentityModulePvid = table.Column<long>(type: "bigint", nullable: false),
                    IdentityAccessLevelPvid = table.Column<long>(type: "bigint", nullable: false),
                    GrantedByIdentityUserPvid = table.Column<long>(type: "bigint", nullable: false),
                    GrantedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleAccess", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleProfiles",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserPvid = table.Column<long>(type: "bigint", nullable: false),
                    IdentityRolePvid = table.Column<long>(type: "bigint", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleProfiles", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserProfiles",
                columns: table => new
                {
                    Pvid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserPvid = table.Column<long>(type: "bigint", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserProfiles", x => x.Pvid);
                });

            migrationBuilder.CreateTable(
                name: "InterestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NRIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Residential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguagePreference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    FirstPasswordReset = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    cid = table.Column<int>(type: "int", nullable: true),
                    referId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordHistory",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PasswordHash1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate3 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate4 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate5 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate6 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordDate7 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CID = table.Column<int>(type: "int", nullable: true),
                    referID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordHistory", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRole",
                        principalColumn: "PvId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRole",
                        principalColumn: "PvId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "IdentityAccessLevel");

            migrationBuilder.DropTable(
                name: "IdentityAccessLevelDetail");

            migrationBuilder.DropTable(
                name: "IdentityAccessLevelExclusive");

            migrationBuilder.DropTable(
                name: "IdentityModule");

            migrationBuilder.DropTable(
                name: "IdentityModuleOrganize");

            migrationBuilder.DropTable(
                name: "IdentityRoleAccess");

            migrationBuilder.DropTable(
                name: "IdentityRoleProfiles");

            migrationBuilder.DropTable(
                name: "IdentityUserProfiles");

            migrationBuilder.DropTable(
                name: "InterestTypes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "UserPasswordHistory");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
