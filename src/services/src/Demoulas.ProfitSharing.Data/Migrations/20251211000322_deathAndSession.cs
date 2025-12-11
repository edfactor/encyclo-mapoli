using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class deathAndSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATE_OF_DEATH",
                table: "DEMOGRAPHIC",
                type: "DATE",
                nullable: true,
                comment: "DateOfDeath");

            migrationBuilder.AddColumn<string>(
                name: "SESSION_ID",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "IS_READ_ONLY", "NAME" },
                values: new object[,]
                {
                    { (byte)11, true, "HR-ReadOnly" },
                    { (byte)12, true, "SSN-Unmasking" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_EVENT_SESSION_ID_CREATEDAT",
                table: "AUDIT_EVENT",
                columns: new[] { "SESSION_ID", "CREATED_AT" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AUDIT_EVENT_SESSION_ID_CREATEDAT",
                table: "AUDIT_EVENT");

            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)11);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)12);

            migrationBuilder.DropColumn(
                name: "DATE_OF_DEATH",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropColumn(
                name: "SESSION_ID",
                table: "AUDIT_EVENT");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);
        }
    }
}
