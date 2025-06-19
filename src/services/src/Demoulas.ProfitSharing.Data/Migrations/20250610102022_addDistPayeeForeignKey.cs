using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDistPayeeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONPAYEE_PAYEEID",
                table: "DISTRIBUTION");

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONPAYEES_PAYEEID",
                table: "DISTRIBUTION",
                column: "PAYEE_ID",
                principalTable: "DISTRIBUTION_PAYEE",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONPAYEES_PAYEEID",
                table: "DISTRIBUTION");

            migrationBuilder.AddForeignKey(
                name: "FK_DISTRIBUTION_DISTRIBUTIONPAYEE_PAYEEID",
                table: "DISTRIBUTION",
                column: "PAYEE_ID",
                principalTable: "DISTRIBUTION_PAYEE",
                principalColumn: "ID");
        }
    }
}
