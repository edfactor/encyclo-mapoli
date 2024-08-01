using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addExecutiveHoursAndDollars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExecutiveEarnings",
                table: "PAYPROFIT",
                type: "DECIMAL(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExecutiveHours",
                table: "PAYPROFIT",
                type: "DECIMAL(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutiveHours",
                table: "PAYPROFIT");

            migrationBuilder.DropColumn(
                name: "ExecutiveEarnings",
                table: "PAYPROFIT");
        }

    }
}
