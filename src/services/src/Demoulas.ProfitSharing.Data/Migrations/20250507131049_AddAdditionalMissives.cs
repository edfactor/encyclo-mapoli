using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalMissives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MISSIVES;");

            migrationBuilder.AlterColumn<string>(
                name: "MESSAGE",
                table: "MISSIVES",
                type: "NVARCHAR2(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AddColumn<string>(
                name: "DESCRIPTION",
                table: "MISSIVES",
                type: "NVARCHAR2(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SEVERITY",
                table: "MISSIVES",
                type: "NVARCHAR2(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.InsertData(
                table: "MISSIVES",
                columns: new[] { "ID", "DESCRIPTION", "MESSAGE", "SEVERITY" },
                values: new object[,]
                {
                                { 1, "The employee has between 2 and 7 years in Profit Sharing, has 1000+ plus hours towards Profit Sharing in the fiscal year, and has company contribution records under the new vesting schedule.", "** VESTING INCREASED ON   CURRENT BALANCE ( > 1000 HRS) **", "Information" },
                                { 2, "The Employee's Zero Contribution Flag is set at 6", "VEST IS NOW 100%, 65+/5 YRS", "Information" },
                                { 3, "Employee is a beneficiary of another employee", "Employee is also a Beneficiary", "Information" },
                                { 4, "The PSN you have entered was not found.  Re-enter using a valid PSN", "Beneficiary not on file", "Error" },
                                { 5, "The Employee Badge Number you have entered is not found.  Re-enter using a valid badge number", "Employee badge not on file", "Error" },
                                { 6, "The Employee SSN you have entered is not on file or you don't have access.  Re-enter using a valid SSN", "Employee SSN not on file", "Error" },
                                { 7, "The Employee's Zero Contribution Flag is set at 7", "*** EMPLOYEE MAY BE 100% - CHECK DATES ***", "Information" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MISSIVES",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "DESCRIPTION",
                table: "MISSIVES");

            migrationBuilder.DropColumn(
                name: "SEVERITY",
                table: "MISSIVES");

            migrationBuilder.AlterColumn<string>(
                name: "MESSAGE",
                table: "MISSIVES",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");
        }
    }
}
