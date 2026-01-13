using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class useBankingInfoTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "BANK_ACCOUNT_SEQ",
                minValue: 1L);

            migrationBuilder.CreateSequence<int>(
                name: "BANK_SEQ",
                minValue: 1L);

            migrationBuilder.CreateSequence<int>(
                name: "PROFIT_SHARE_CHECK_NUMBER_SEQ",
                minValue: 1L);

            migrationBuilder.AddColumn<Guid>(
                name: "CHECK_RUN_WORKFLOW_ID",
                table: "PROFIT_SHARE_CHECK",
                type: "RAW(16)",
                nullable: true);

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
                name: "BANK",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "BANK_SEQ.NEXTVAL"),
                    ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
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
                    IS_DISABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    CREATEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MODIFIEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK", x => x.ID);
                });

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
                    FED_ACH_CHANGE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    FEDWIRE_REVISION_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    NOTES = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    EFFECTIVE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    DISCONTINUED_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    CREATEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MODIFIEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                columns: new[] { "ID", "CITY", "CREATED_AT_UTC", "CREATEDBY", "FEDACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "MODIFIED_AT_UTC", "MODIFIEDBY", "NAME", "OFFICE_TYPE", "PHONE", "ROUTING_NUMBER", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER", "STATE", "STATUS" },
                values: new object[] { 1, "Lake Success", new DateTimeOffset(new DateTime(2026, 1, 12, 19, 7, 50, 236, DateTimeKind.Unspecified).AddTicks(6625), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami, FL", new DateTime(2023, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "NEWTEK BANK, NA", null, null, "Newtek Bank, NA", "Main Office", "516-254-7586", "026004297", "100 Orchard Street, East Rutherford, NJ", "021001208", "NY", "Active" });

            migrationBuilder.InsertData(
                table: "BANK_ACCOUNT",
                columns: new[] { "ID", "ACCOUNT_NAME", "ACCOUNT_NUMBER", "BANK_ID", "CREATED_AT_UTC", "CREATEDBY", "DISCONTINUED_DATE", "EFFECTIVE_DATE", "FED_ACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "IS_PRIMARY", "MODIFIED_AT_UTC", "MODIFIEDBY", "NOTES", "ROUTING_NUMBER", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER" },
                values: new object[] { 1, "Profit Sharing Distribution Account", "PLACEHOLDER", 1, new DateTimeOffset(new DateTime(2026, 1, 12, 19, 7, 50, 242, DateTimeKind.Unspecified).AddTicks(6447), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, null, null, null, null, true, null, null, null, "026004297", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK",
                column: "CHECK_RUN_WORKFLOW_ID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_SHARE_CHECK_CHECK_RUN_WORKFLOW_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK",
                column: "CHECK_RUN_WORKFLOW_ID",
                principalTable: "CHECK_RUN_WORKFLOW",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_SHARE_CHECK_CHECK_RUN_WORKFLOW_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropTable(
                name: "BANK_ACCOUNT");

            migrationBuilder.DropTable(
                name: "BANK");

            migrationBuilder.DropIndex(
                name: "IX_PROFIT_SHARE_CHECK_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropColumn(
                name: "CHECK_RUN_WORKFLOW_ID",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropSequence(
                name: "BANK_ACCOUNT_SEQ");

            migrationBuilder.DropSequence(
                name: "BANK_SEQ");

            migrationBuilder.DropSequence(
                name: "PROFIT_SHARE_CHECK_NUMBER_SEQ");

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
