using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class adjustPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_HISTORY_DEMOGRAPHIC_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.AlterColumn<decimal>(
                name: "CONTRIBUTION",
                table: "PROFIT_DETAIL",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                comment: "Contribution to plan from DMB",
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(9,2)",
                oldPrecision: 9,
                oldScale: 2);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CONTRIBUTION",
                table: "PROFIT_DETAIL",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(9,2)",
                oldPrecision: 9,
                oldScale: 2,
                oldComment: "Contribution to plan from DMB");

           
            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_HISTORY_DEMOGRAPHIC_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY",
                column: "DEMOGRAPHIC_ID",
                principalTable: "DEMOGRAPHIC",
                principalColumn: "ID");
        }
    }
}
