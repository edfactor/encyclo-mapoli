using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addProfitDetailCreatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATEDUTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATEDUTC",
                table: "PROFIT_DETAIL");
        }
    }
}
