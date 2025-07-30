using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class reportCheckSum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.CreateTable(
                name: "REPORT_CHECKSUM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    REPORT_TYPE = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    REQUEST_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    REPORT_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    KEYFIELDS_CHECKSUM_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_CHECKSUM", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REPORT_CHECKSUM");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "TIMESTAMP WITH TIME ZONE",
                oldNullable: true,
                oldDefaultValueSql: "SYSTIMESTAMP");
        }
    }
}
