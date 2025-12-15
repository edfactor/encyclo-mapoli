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

            // REMOVED DUPLICATE InsertData - IDs 11 and 12 were already inserted in previous migration

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

            // REMOVED DUPLICATE DeleteData - IDs 11 and 12 are managed by previous migration

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
