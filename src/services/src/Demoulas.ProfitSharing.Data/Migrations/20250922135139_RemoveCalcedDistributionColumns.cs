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
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUS_STATUSID",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "CHECK_AMOUNT",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "FEDERAL_TAX_PERCENTAGE",
                table: "DISTRIBUTION");

            migrationBuilder.DropColumn(
                name: "STATE_TAX_PERCENTAGE",
                table: "DISTRIBUTION");

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUSES_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID",
                principalTable: "DISTRIBUTION_STATUS",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUSES_STATUSID",
                table: "DISTRIBUTION");

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

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUS_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID",
                principalTable: "DISTRIBUTION_STATUS",
                principalColumn: "ID");
        }
    }
}
