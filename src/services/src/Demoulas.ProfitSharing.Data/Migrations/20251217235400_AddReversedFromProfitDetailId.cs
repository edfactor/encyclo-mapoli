using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReversedFromProfitDetailId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "REVERSED_FROM_PROFIT_DETAIL_ID",
                table: "PROFIT_DETAIL",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_REVERSED_FROM_PROFIT_DETAIL_ID",
                table: "PROFIT_DETAIL",
                column: "REVERSED_FROM_PROFIT_DETAIL_ID",
                filter: "REVERSED_FROM_PROFIT_DETAIL_ID IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_DETAIL_PROFIT_DETAIL_REVERSEDFROMPROFITDETAILID",
                table: "PROFIT_DETAIL",
                column: "REVERSED_FROM_PROFIT_DETAIL_ID",
                principalTable: "PROFIT_DETAIL",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_DETAIL_PROFIT_DETAIL_REVERSEDFROMPROFITDETAILID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropIndex(
                name: "IX_PROFIT_DETAIL_REVERSED_FROM_PROFIT_DETAIL_ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "REVERSED_FROM_PROFIT_DETAIL_ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);
        }
    }
}
