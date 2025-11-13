using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedColumnsInPayProfit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_TERMINATIONCODE_TERMINATIONCODEID",
                table: "DEMOGRAPHIC");

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "TOTAL_HOURS",
                table: "PAY_PROFIT",
                type: "DECIMAL(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                computedColumnSql: "HOURS_EXECUTIVE + CURRENT_HOURS_YEAR",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TOTAL_INCOME",
                table: "PAY_PROFIT",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                computedColumnSql: "INCOME_EXECUTIVE + CURRENT_INCOME_YEAR",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_TOTALHOURS",
                table: "PAY_PROFIT",
                columns: new[] { "PROFIT_YEAR", "TOTAL_HOURS" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_VALIDFROM_VALIDTO_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY",
                columns: new[] { "VALID_FROM", "VALID_TO", "DEMOGRAPHIC_ID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_TERMINATIONCODES_TERMINATIONCODEID",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_CODE_ID",
                principalTable: "TERMINATION_CODE",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_TERMINATIONCODES_TERMINATIONCODEID",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_TOTALHOURS",
                table: "PAY_PROFIT");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_VALIDFROM_VALIDTO_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "TOTAL_HOURS",
                table: "PAY_PROFIT");

            migrationBuilder.DropColumn(
                name: "TOTAL_INCOME",
                table: "PAY_PROFIT");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_TERMINATIONCODE_TERMINATIONCODEID",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_CODE_ID",
                principalTable: "TERMINATION_CODE",
                principalColumn: "ID");
        }
    }
}
