using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BANK",
                table: "BANK");

            migrationBuilder.DeleteData(
                table: "BANK",
                keyColumn: "ROUTING_NUMBER",
                keyValue: "026004297");

            migrationBuilder.DropColumn(
                name: "ACCOUNT_NUMBER",
                table: "BANK");

            migrationBuilder.CreateSequence<int>(
                name: "BANK_ACCOUNT_SEQ",
                minValue: 1L);

            migrationBuilder.CreateSequence<int>(
                name: "BANK_SEQ",
                minValue: 1L);

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<string>(
                name: "ROUTING_NUMBER",
                table: "BANK",
                type: "NVARCHAR2(9)",
                maxLength: 9,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(9)",
                oldMaxLength: 9);

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "BANK",
                type: "NUMBER(10)",
                nullable: false,
                defaultValueSql: "BANK_SEQ.NEXTVAL");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT_UTC",
                table: "BANK",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "CREATED_BY",
                table: "BANK",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IS_DISABLED",
                table: "BANK",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "BANK",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "BANK",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BANK",
                table: "BANK",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "BANK_ACCOUNT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "BANK_ACCOUNT_SEQ.NEXTVAL"),
                    BANK_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    ACCOUNT_NUMBER = table.Column<string>(type: "NVARCHAR2(34)", maxLength: 34, nullable: false),
                    ACCOUNT_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    IS_PRIMARY = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    IS_DISABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    SERVICING_FED_ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SERVICING_FED_ADDRESS = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    FEDWIRE_TELEGRAPHIC_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FEDWIRE_LOCATION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FED_ACH_CHANGE_DATE = table.Column<DateOnly>(type: "DATE", nullable: true),
                    FEDWIRE_REVISION_DATE = table.Column<DateOnly>(type: "DATE", nullable: true),
                    NOTES = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    EFFECTIVE_DATE = table.Column<DateOnly>(type: "DATE", nullable: true),
                    DISCONTINUED_DATE = table.Column<DateOnly>(type: "DATE", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    CREATED_BY = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
                    MODIFIED_BY = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK_ACCOUNT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BANK_ACCOUNT_BANK_BANKID",
                        column: x => x.BANK_ID,
                        principalTable: "BANK",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "BANK",
                columns: new[] { "ID", "CITY", "CREATED_AT_UTC", "CREATED_BY", "FEDACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "MODIFIED_AT_UTC", "MODIFIED_BY", "NAME", "OFFICE_TYPE", "PHONE", "ROUTING_NUMBER", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER", "STATE", "STATUS" },
                values: new object[] { 1, "Lake Success", new DateTimeOffset(new DateTime(2026, 1, 12, 2, 42, 57, 678, DateTimeKind.Unspecified).AddTicks(313), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami, FL", new DateTime(2023, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "NEWTEK BANK, NA", null, null, "Newtek Bank, NA", "Main Office", "516-254-7586", "026004297", "100 Orchard Street, East Rutherford, NJ", "021001208", "NY", "Active" });

            migrationBuilder.InsertData(
                table: "BANK_ACCOUNT",
                columns: new[] { "ID", "ACCOUNT_NAME", "ACCOUNT_NUMBER", "BANK_ID", "CREATED_AT_UTC", "CREATED_BY", "IS_PRIMARY", "MODIFIED_AT_UTC", "MODIFIED_BY", "ROUTING_NUMBER" },
                values: new object[] { 1, "Profit Sharing Distribution Account", "PLACEHOLDER", 1, new DateTimeOffset(new DateTime(2026, 1, 12, 2, 42, 57, 681, DateTimeKind.Unspecified).AddTicks(9552), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", true, null, null, "026004297" });

            migrationBuilder.CreateIndex(
                name: "IX_BANK_IS_DISABLED",
                table: "BANK",
                column: "IS_DISABLED");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_NAME",
                table: "BANK",
                column: "NAME");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ROUTING_NUMBER",
                table: "BANK",
                column: "ROUTING_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_BANK_ID",
                table: "BANK_ACCOUNT",
                column: "BANK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_BANK_PRIMARY",
                table: "BANK_ACCOUNT",
                columns: new[] { "BANK_ID", "IS_PRIMARY" });

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_IS_DISABLED",
                table: "BANK_ACCOUNT",
                column: "IS_DISABLED");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_ROUTING_NUMBER",
                table: "BANK_ACCOUNT",
                column: "ROUTING_NUMBER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BANK_ACCOUNT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BANK",
                table: "BANK");

            migrationBuilder.DropIndex(
                name: "IX_BANK_IS_DISABLED",
                table: "BANK");

            migrationBuilder.DropIndex(
                name: "IX_BANK_NAME",
                table: "BANK");

            migrationBuilder.DropIndex(
                name: "IX_BANK_ROUTING_NUMBER",
                table: "BANK");

            migrationBuilder.DeleteData(
                table: "BANK",
                keyColumn: "ID",
                keyColumnType: "NUMBER(10)",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "ID",
                table: "BANK");

            migrationBuilder.DropColumn(
                name: "CREATED_AT_UTC",
                table: "BANK");

            migrationBuilder.DropColumn(
                name: "CREATED_BY",
                table: "BANK");

            migrationBuilder.DropColumn(
                name: "IS_DISABLED",
                table: "BANK");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "BANK");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "BANK");

            migrationBuilder.DropSequence(
                name: "BANK_ACCOUNT_SEQ");

            migrationBuilder.DropSequence(
                name: "BANK_SEQ");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "ROUTING_NUMBER",
                table: "BANK",
                type: "NVARCHAR2(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(9)",
                oldMaxLength: 9,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ACCOUNT_NUMBER",
                table: "BANK",
                type: "NVARCHAR2(34)",
                maxLength: 34,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BANK",
                table: "BANK",
                column: "ROUTING_NUMBER");

            migrationBuilder.InsertData(
                table: "BANK",
                columns: new[] { "ROUTING_NUMBER", "ACCOUNT_NUMBER", "CITY", "FEDACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "NAME", "OFFICE_TYPE", "PHONE", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER", "STATE", "STATUS" },
                values: new object[] { "026004297", null, "Lake Success", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami, FL", new DateTime(2023, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "NEWTEK BANK, NA", "Newtek Bank, NA", "Main Office", "516-254-7586", "100 Orchard Street, East Rutherford, NJ", "021001208", "NY", "Active" });
        }
    }
}
