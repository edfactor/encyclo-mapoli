using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCalcedDistributionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.DropColumn(
                name: "CHECK_AMOUNT",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "FEDERAL_TAX_PERCENTAGE",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "STATE_TAX_PERCENTAGE",
                table: "DISTRIBUTION");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CHECK_AMOUNT",
                table: "DISTRIBUTION",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FEDERAL_TAX_PERCENTAGE",
                table: "DISTRIBUTION",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "STATE_TAX_PERCENTAGE",
                table: "DISTRIBUTION",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
