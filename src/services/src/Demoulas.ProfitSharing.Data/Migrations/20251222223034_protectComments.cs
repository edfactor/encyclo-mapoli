using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class protectComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "STATE_TAX",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "REPORT_CHECKSUM",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "COMMENT_TYPE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "ANNUITY_RATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)2,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)3,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)4,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)5,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)6,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)7,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)8,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)9,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)10,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)11,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)12,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)13,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)14,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)15,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)16,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)17,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)18,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)19,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)20,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)21,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)22,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)23,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)24,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)26,
                columns: new[] { "ISPROTECTED", "MODIFIED_AT_UTC" },
                values: new object[] { true, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)27,
                column: "MODIFIED_AT_UTC",
                value: null);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)28,
                column: "MODIFIED_AT_UTC",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "STATE_TAX",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "REPORT_CHECKSUM",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "COMMENT_TYPE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "ANNUITY_RATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)2,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)3,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)4,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)5,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)6,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)7,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)8,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)9,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)10,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)11,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)12,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)14,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)15,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)19,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)21,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)22,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)23,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)24,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25,
                column: "ISPROTECTED",
                value: false);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)26,
                column: "ISPROTECTED",
                value: false);
        }
    }
}
