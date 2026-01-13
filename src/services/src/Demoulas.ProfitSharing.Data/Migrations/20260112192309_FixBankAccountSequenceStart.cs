using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBankAccountSequenceStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix: Restart sequences at 2 since seed data uses ID=1
            migrationBuilder.RestartSequence(
                name: "BANK_SEQ",
                startValue: 2L);

            migrationBuilder.RestartSequence(
                name: "BANK_ACCOUNT_SEQ",
                startValue: 2L);

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.UpdateData(
                table: "BANK",
                keyColumn: "ID",
                keyValue: 1,
                column: "CREATED_AT_UTC",
                value: new DateTimeOffset(new DateTime(2026, 1, 12, 19, 23, 7, 777, DateTimeKind.Unspecified).AddTicks(306), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "BANK_ACCOUNT",
                keyColumn: "ID",
                keyValue: 1,
                column: "CREATED_AT_UTC",
                value: new DateTimeOffset(new DateTime(2026, 1, 12, 19, 23, 7, 782, DateTimeKind.Unspecified).AddTicks(9075), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert sequences back to starting at 1
            migrationBuilder.RestartSequence(
                name: "BANK_SEQ",
                startValue: 1L);

            migrationBuilder.RestartSequence(
                name: "BANK_ACCOUNT_SEQ",
                startValue: 1L);

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.UpdateData(
                table: "BANK",
                keyColumn: "ID",
                keyValue: 1,
                column: "CREATED_AT_UTC",
                value: new DateTimeOffset(new DateTime(2026, 1, 12, 19, 7, 50, 236, DateTimeKind.Unspecified).AddTicks(6625), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "BANK_ACCOUNT",
                keyColumn: "ID",
                keyValue: 1,
                column: "CREATED_AT_UTC",
                value: new DateTimeOffset(new DateTime(2026, 1, 12, 19, 7, 50, 242, DateTimeKind.Unspecified).AddTicks(6447), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
