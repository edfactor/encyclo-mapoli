using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addYearsOfServiceCreditColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YEARS_IN_PLAN",
                table: "PAY_PROFIT");

            migrationBuilder.AddColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.Sql(@"UPDATE PROFIT_DETAIL pd 
SET pd.YEARS_OF_SERVICE_CREDIT = CASE WHEN pd.Contribution > 0 OR pd.COMMENT_TYPE_ID IN (5, 16, 17, 18, 19, 24) THEN 1 ELSE 0 END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL");

            migrationBuilder.AddColumn<byte>(
                name: "YEARS_IN_PLAN",
                table: "PAY_PROFIT",
                type: "NUMBER(2)",
                precision: 2,
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
