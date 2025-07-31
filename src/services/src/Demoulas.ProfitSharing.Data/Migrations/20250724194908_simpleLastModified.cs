using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class simpleLastModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LAST_UPDATE",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHIC");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "PROFIT_DETAIL",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "PAY_PROFIT",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "DISTRIBUTION_REQUEST",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "DISTRIBUTION",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "DEMOGRAPHIC",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "BENEFICIARY_CONTACT",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "BENEFICIARY",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "DISTRIBUTION_REQUEST");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION_REQUEST");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "DISTRIBUTION_REQUEST");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "BENEFICIARY_CONTACT");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_CONTACT");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "BENEFICIARY_CONTACT");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "BENEFICIARY");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LAST_UPDATE",
                table: "PAY_PROFIT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHIC",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSDATE");
        }
    }
}
