using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRmdsFactorByAgeTable : Migration
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

            migrationBuilder.CreateTable(
                name: "RMDS_FACTOR_BY_AGE",
                columns: table => new
                {
                    AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    FACTOR = table.Column<decimal>(type: "DECIMAL(4,1)", precision: 4, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RMDS_FACTOR_BY_AGE", x => x.AGE);
                });

            migrationBuilder.InsertData(
                table: "RMDS_FACTOR_BY_AGE",
                columns: new[] { "AGE", "FACTOR" },
                values: new object[,]
                {
                    { (byte)73, 26.5m },
                    { (byte)74, 25.5m },
                    { (byte)75, 24.6m },
                    { (byte)76, 23.7m },
                    { (byte)77, 22.9m },
                    { (byte)78, 22.0m },
                    { (byte)79, 21.1m },
                    { (byte)80, 20.2m },
                    { (byte)81, 19.4m },
                    { (byte)82, 18.5m },
                    { (byte)83, 17.7m },
                    { (byte)84, 16.8m },
                    { (byte)85, 16.0m },
                    { (byte)86, 15.2m },
                    { (byte)87, 14.4m },
                    { (byte)88, 13.7m },
                    { (byte)89, 12.9m },
                    { (byte)90, 12.2m },
                    { (byte)91, 11.5m },
                    { (byte)92, 10.8m },
                    { (byte)93, 10.1m },
                    { (byte)94, 9.5m },
                    { (byte)95, 8.9m },
                    { (byte)96, 8.4m },
                    { (byte)97, 7.8m },
                    { (byte)98, 7.3m },
                    { (byte)99, 6.8m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RMDS_FACTOR_BY_AGE");

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
