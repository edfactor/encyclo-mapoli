using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBankTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BANK",
                columns: table => new
                {
                    ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    OFFICE_TYPE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true),
                    PHONE = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    FEDACH_CHANGE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    SERVICING_FED_ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SERVICING_FED_ADDRESS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    FEDWIRE_TELEGRAPHIC_NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    FEDWIRE_LOCATION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FEDWIRE_REVISION_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    ACCOUNT_NUMBER = table.Column<string>(type: "NVARCHAR2(34)", maxLength: 34, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK", x => x.ROUTING_NUMBER);
                });

            migrationBuilder.InsertData(
                table: "BANK",
                columns: new[] { "ROUTING_NUMBER", "ACCOUNT_NUMBER", "CITY", "FEDACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "NAME", "OFFICE_TYPE", "PHONE", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER", "STATE", "STATUS" },
                values: new object[] { "026004297", null, "Lake Success", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami, FL", new DateTime(2023, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "NEWTEK BANK, NA", "Newtek Bank, NA", "Main Office", "516-254-7586", "100 Orchard Street, East Rutherford, NJ", "021001208", "NY", "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BANK");
        }
    }
}
