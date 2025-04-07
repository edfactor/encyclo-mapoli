using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeadColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EARNINGS_ETVA_VALUE",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "SECONDARY_EARNINGS",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "SECONDARY_ETVA_EARNINGS",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "AMOUNT",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "DISTRIBUTION",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "EARNINGS",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "SECONDARY_EARNINGS",
                table: "BENEFICIARY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EARNINGS_ETVA_VALUE",
                table: "PAY_PROFIT",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SECONDARY_EARNINGS",
                table: "PAY_PROFIT",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SECONDARY_ETVA_EARNINGS",
                table: "PAY_PROFIT",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AMOUNT",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DISTRIBUTION",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EARNINGS",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SECONDARY_EARNINGS",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
