using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "NAVIGATION_ROLE_JSON",
                table: "NAVIGATION");

            migrationBuilder.AlterColumn<byte>(
                name: "STATUS_ID",
                table: "NAVIGATION_TRACKING",
                type: "NUMBER(3)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED",
                table: "NAVIGATION_TRACKING",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NAME",
                table: "NAVIGATION_STATUS",
                type: "NVARCHAR2(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "ID",
                table: "NAVIGATION_STATUS",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)")
                .OldAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AlterColumn<byte>(
                name: "STATUS_ID",
                table: "NAVIGATION",
                type: "NUMBER(3)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NAVIGATION_ROLE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    URL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_ROLE", x => x.ID);
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
                name: "IX_NAVIGATIONNAVIGATIONROLE_REQUIREDROLESID",
                table: "NAVIGATIONNAVIGATIONROLE",
                column: "REQUIREDROLESID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.DropTable(
                name: "NAVIGATION_ROLE");

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_STATUS",
                keyColumn: "ID",
                keyValue: (byte)4);

            migrationBuilder.AlterColumn<int>(
                name: "STATUS_ID",
                table: "NAVIGATION_TRACKING",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED",
                table: "NAVIGATION_TRACKING",
                type: "TIMESTAMP(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NAME",
                table: "NAVIGATION_STATUS",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "NAVIGATION_STATUS",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)")
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AlterColumn<int>(
                name: "STATUS_ID",
                table: "NAVIGATION",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NAVIGATION_ROLE_JSON",
                table: "NAVIGATION",
                type: "NVARCHAR2(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.InsertData(
                table: "NAVIGATION_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { 1, "Not Started" },
                    { 2, "In Progress" },
                    { 3, "Blocked" },
                    { 4, "Successful" }
                });
        }
    }
}
