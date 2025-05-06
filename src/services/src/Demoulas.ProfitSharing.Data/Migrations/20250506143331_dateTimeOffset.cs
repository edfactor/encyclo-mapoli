using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class dateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UPDATED_DATE",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LAST_UPDATE",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LAST_MODIFIED",
                table: "NAVIGATION_TRACKING",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "STARTED",
                table: "JOB",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "COMPLETED",
                table: "JOB",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "AS_OF_DATETIME",
                table: "FROZEN_STATE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CREATED",
                table: "DEMOGRAPHIC_SYNC_AUDIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "VALID_TO",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "VALID_FROM",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CREATED_DATETIME",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IMPORT_DATE_TIME_UTC",
                table: "DATA_IMPORT_RECORD",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP(7) WITH TIME ZONE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "YE_UPDATE_STATUS",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_UPDATE",
                table: "PAY_PROFIT",
                type: "TIMESTAMP",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED",
                table: "NAVIGATION_TRACKING",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "STARTED",
                table: "JOB",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "COMPLETED",
                table: "JOB",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE",
                type: "DATE",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AS_OF_DATETIME",
                table: "FROZEN_STATE",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED",
                table: "DEMOGRAPHIC_SYNC_AUDIT",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VALID_TO",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VALID_FROM",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATETIME",
                table: "DEMOGRAPHIC_HISTORY",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP(7) WITH TIME ZONE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHIC",
                type: "DATE",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IMPORT_DATE_TIME_UTC",
                table: "DATA_IMPORT_RECORD",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE");
        }
    }
}
