using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NAVIGATION_ROLE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    URL = table.Column<string>(type: "NVARCHAR2(65)", maxLength: 65, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_ROLE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_STATUS",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PARENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TITLE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SUB_TITLE = table.Column<string>(type: "NVARCHAR2(70)", maxLength: 70, nullable: true),
                    URL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    STATUS_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    ORDER_NUMBER = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ICON = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATION_NAVIGATIONSTATUSES_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "NAVIGATION_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATION_NAVIGATION_PARENTID",
                        column: x => x.PARENT_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_TRACKING",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAVIGATION_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STATUS_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: true),
                    LAST_MODIFIED = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_TRACKING", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATION_TRACKING_NAVIGATION_NAVIGATIONID",
                        column: x => x.NAVIGATION_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATION_TRACKING_NAVIGATION_STATUS_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "NAVIGATION_STATUS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATIONNAVIGATIONROLE",
                columns: table => new
                {
                    NAVIGATIONID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    REQUIREDROLESID = table.Column<byte>(type: "NUMBER(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATIONNAVIGATIONROLE", x => new { x.NAVIGATIONID, x.REQUIREDROLESID });
                    table.ForeignKey(
                        name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATIONROLES_REQUIREDROLESID",
                        column: x => x.REQUIREDROLESID,
                        principalTable: "NAVIGATION_ROLE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATION_NAVIGATIONID",
                        column: x => x.NAVIGATIONID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "URL" },
                values: new object[,]
                {
                    { (byte)1, "Profit-Sharing-Administrator" },
                    { (byte)2, "Finance-Manager" },
                    { (byte)3, "Distributions-Clerk" },
                    { (byte)4, "Hardship-Administrator" },
                    { (byte)5, "Impersonation" },
                    { (byte)6, "IT-Operations" }
                });

            migrationBuilder.InsertData(
                table: "NAVIGATION_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "Not Started" },
                    { (byte)2, "In Progress" },
                    { (byte)3, "Blocked" },
                    { (byte)4, "Successful" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_PARENTID",
                table: "NAVIGATION",
                column: "PARENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_STATUS_ID",
                table: "NAVIGATION",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_TRACKING_NAVIGATIONID",
                table: "NAVIGATION_TRACKING",
                column: "NAVIGATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_TRACKING_STATUS_ID",
                table: "NAVIGATION_TRACKING",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONNAVIGATIONROLE_REQUIREDROLESID",
                table: "NAVIGATIONNAVIGATIONROLE",
                column: "REQUIREDROLESID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NAVIGATION_TRACKING");

            migrationBuilder.DropTable(
                name: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.DropTable(
                name: "NAVIGATION_ROLE");

            migrationBuilder.DropTable(
                name: "NAVIGATION");

            migrationBuilder.DropTable(
                name: "NAVIGATION_STATUS");
        }
    }
}
