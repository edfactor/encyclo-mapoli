using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class lastModifiedUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "PROFIT_DETAIL",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "PAY_PROFIT",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "DISTRIBUTION_REQUEST",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "DISTRIBUTION",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                newName: "MODIFIED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "DEMOGRAPHIC",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                newName: "MODIFIED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "BENEFICIARY_CONTACT",
                newName: "CREATED_AT_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "BENEFICIARY",
                newName: "CREATED_AT_UTC");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY");

            migrationBuilder.DropColumn(
                name: "CREATED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "PROFIT_DETAIL",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "PAY_PROFIT",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "DISTRIBUTION_REQUEST",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "DISTRIBUTION",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "MODIFIED_AT_UTC",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                newName: "CREATED_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "DEMOGRAPHIC",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "MODIFIED_AT_UTC",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                newName: "CREATED_UTC");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "BENEFICIARY_CONTACT",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT_UTC",
                table: "BENEFICIARY",
                newName: "CREATED_AT");
        }
    }
}
