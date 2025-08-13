using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeRedundentColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_UTC",
                table: "PROFIT_DETAIL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_UTC",
                table: "PROFIT_DETAIL",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");
        }
    }
}
