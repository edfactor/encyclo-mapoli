using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class consistentColumns1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UPDATED_BY",
                table: "YE_UPDATE_STATUS");

            migrationBuilder.DropColumn(
                name: "UPDATED_DATE",
                table: "YE_UPDATE_STATUS");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT_UTC",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "YE_UPDATE_STATUS",
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
                table: "YE_UPDATE_STATUS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "YE_UPDATE_STATUS");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "YE_UPDATE_STATUS");

            migrationBuilder.AddColumn<string>(
                name: "UPDATED_BY",
                table: "YE_UPDATE_STATUS",
                type: "NVARCHAR2(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UPDATED_DATE",
                table: "YE_UPDATE_STATUS",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
