using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addFreezeCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AS_OF_DATETIME",
                table: "FROZEN_STATE",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE",
                type: "DATE",
                nullable: false,
                defaultValueSql: "SYSDATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_DATETIME",
                table: "FROZEN_STATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AS_OF_DATETIME",
                table: "FROZEN_STATE",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");
        }
    }
}
