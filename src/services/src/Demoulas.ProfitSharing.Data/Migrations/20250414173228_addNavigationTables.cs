using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNavigationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NAVIGATIONROLES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATIONROLES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATIONSTATUSES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATIONSTATUSES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PARENTID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TITLE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SUBTITLE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    URL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STATUSID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NAVIGATIONROLEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ORDERNUMBER = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ICON = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATIONS_NAVIGATIONROLES_NAVIGATIONROLEID",
                        column: x => x.NAVIGATIONROLEID,
                        principalTable: "NAVIGATIONROLES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATIONS_NAVIGATIONSTATUSES_STATUSID",
                        column: x => x.STATUSID,
                        principalTable: "NAVIGATIONSTATUSES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATIONS_NAVIGATIONS_PARENTID",
                        column: x => x.PARENTID,
                        principalTable: "NAVIGATIONS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATIONTRACKINGS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAVIGATIONID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STATUSID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LASTMODIFIED = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATIONTRACKINGS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATIONTRACKINGS_NAVIGATIONSTATUSES_STATUSID",
                        column: x => x.STATUSID,
                        principalTable: "NAVIGATIONSTATUSES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATIONTRACKINGS_NAVIGATIONS_NAVIGATIONID",
                        column: x => x.NAVIGATIONID,
                        principalTable: "NAVIGATIONS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONS_NAVIGATIONROLEID",
                table: "NAVIGATIONS",
                column: "NAVIGATIONROLEID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONS_PARENTID",
                table: "NAVIGATIONS",
                column: "PARENTID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONS_STATUSID",
                table: "NAVIGATIONS",
                column: "STATUSID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONTRACKINGS_NAVIGATIONID",
                table: "NAVIGATIONTRACKINGS",
                column: "NAVIGATIONID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATIONTRACKINGS_STATUSID",
                table: "NAVIGATIONTRACKINGS",
                column: "STATUSID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NAVIGATIONTRACKINGS");

            migrationBuilder.DropTable(
                name: "NAVIGATIONS");

            migrationBuilder.DropTable(
                name: "NAVIGATIONROLES");

            migrationBuilder.DropTable(
                name: "NAVIGATIONSTATUSES");
        }
    }
}
