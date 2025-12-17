using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBeneficiaryKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARY_KIND_KINDID",
                table: "BENEFICIARY");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_KIND");

            migrationBuilder.DropIndex(
                name: "IX_BENEFICIARY_KINDID",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "KIND_ID",
                table: "BENEFICIARY_ARCHIVE");

            migrationBuilder.DropColumn(
                name: "KIND_ID",
                table: "BENEFICIARY");

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "IS_READ_ONLY", "NAME" },
                values: new object[,]
                {
                    { (byte)11, true, "HR-ReadOnly" },
                    { (byte)12, true, "SSN-Unmasking" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)11);

            migrationBuilder.DeleteData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)12);

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "KIND_ID",
                table: "BENEFICIARY_ARCHIVE",
                type: "NVARCHAR2(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KIND_ID",
                table: "BENEFICIARY",
                type: "NVARCHAR2(1)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_KIND",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_KIND", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "BENEFICIARY_KIND",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "P", "Primary" },
                    { "S", "Secondary" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_KINDID",
                table: "BENEFICIARY",
                column: "KIND_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARY_KIND_KINDID",
                table: "BENEFICIARY",
                column: "KIND_ID",
                principalTable: "BENEFICIARY_KIND",
                principalColumn: "ID");
        }
    }
}
