using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class FactorOutAccountFromThirdPartyPayee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACCOUNT",
                table: "DISTRIBUTION_THIRDPARTY_PAYEE");

            migrationBuilder.AddColumn<string>(
                name: "THIRD_PARTY_PAYEE_ACCOUNT",
                table: "DISTRIBUTION",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "THIRD_PARTY_PAYEE_ACCOUNT",
                table: "DISTRIBUTION");

            migrationBuilder.AddColumn<string>(
                name: "ACCOUNT",
                table: "DISTRIBUTION_THIRDPARTY_PAYEE",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true);
        }
    }
}
