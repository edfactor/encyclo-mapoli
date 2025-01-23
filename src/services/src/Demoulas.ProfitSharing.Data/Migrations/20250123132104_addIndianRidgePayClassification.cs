using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIndianRidgePayClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ETVA",
                table: "PAY_PROFIT",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            

            migrationBuilder.InsertData(
                table: "PAY_CLASSIFICATION",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)99, "Indian Ridge" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: (byte)99);

            migrationBuilder.AlterColumn<decimal>(
                name: "ETVA",
                table: "PAY_PROFIT",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(9,2)",
                oldPrecision: 9,
                oldScale: 2);
        }
    }
}
