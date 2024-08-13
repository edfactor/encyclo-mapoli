using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesForImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARYTYPE_BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropForeignKey(
                name: "FK_PROFITDETAIL_PROFITCODE_PROFIT_CODE_ID",
                table: "PROFITDETAIL");

            migrationBuilder.DropForeignKey(
                name: "FK_PROFITDETAIL_TAXCODE_TAX_CODE_ID",
                table: "PROFITDETAIL");

            migrationBuilder.DropIndex(
                name: "IX_BENEFICIARY_BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PROFITDETAIL",
                table: "PROFITDETAIL");

            migrationBuilder.DropColumn(
                name: "BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "COMMENT",
                table: "PROFITDETAIL");

            migrationBuilder.DropColumn(
                name: "MONTH",
                table: "PROFITDETAIL");

            migrationBuilder.DropColumn(
                name: "PROFIT_CLIENT",
                table: "PROFITDETAIL");

            migrationBuilder.DropColumn(
                name: "YEAR",
                table: "PROFITDETAIL");

            migrationBuilder.RenameTable(
                name: "PROFITDETAIL",
                newName: "PROFIT_DETAIL");

            migrationBuilder.RenameColumn(
                name: "PROF_DIST_ID",
                table: "PROFIT_DETAIL",
                newName: "DISTRIBUTION_SEQUENCE");

            migrationBuilder.RenameIndex(
                name: "IX_PROFITDETAIL_TAX_CODE_ID",
                table: "PROFIT_DETAIL",
                newName: "IX_PROFIT_DETAIL_TAX_CODE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_PROFITDETAIL_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                newName: "IX_PROFIT_DETAIL_PROFIT_CODE_ID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHICS",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 12, 17, 28, 33, 3, DateTimeKind.Local).AddTicks(3230),
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValue: new DateTime(2024, 8, 5, 14, 37, 1, 235, DateTimeKind.Local).AddTicks(1415));

            migrationBuilder.AddColumn<decimal>(
                name: "AMOUNT",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DISTRIBUTION",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EARNINGS",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "KIND_ID",
                table: "BENEFICIARY",
                type: "NVARCHAR2(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SECONDARY_EARNINGS",
                table: "BENEFICIARY",
                type: "DECIMAL(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "ZEROCONT",
                table: "PROFIT_DETAIL",
                type: "NVARCHAR2(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "PROFIT_DETAIL",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0)
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddColumn<byte>(
                name: "MONTH_TO_DATE",
                table: "PROFIT_DETAIL",
                type: "NUMBER(2,0)",
                precision: 2,
                scale: 0,
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "REMARK",
                table: "PROFIT_DETAIL",
                type: "NVARCHAR2(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "YEAR_TO_DATE",
                table: "PROFIT_DETAIL",
                type: "NUMBER(4,0)",
                precision: 4,
                scale: 0,
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROFIT_DETAIL",
                table: "PROFIT_DETAIL",
                column: "ID");

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

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_FREQUENCY",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_FREQUENCY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_STATUS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_REL_PERCENT",
                columns: table => new
                {
                    PSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    KIND_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_REL_PERCENT", x => x.PSN);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_REL_PERCENT_BENEFICIARY_KIND_KINDID",
                        column: x => x.KIND_ID,
                        principalTable: "BENEFICIARY_KIND",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION",
                columns: table => new
                {
                    SSN = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    SEQUENCE_NUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EMPLOYEE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false),
                    FREQUENCY_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    PAYEE_SSN = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PAYEE_NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    THIRD_PARTY_PAYEEE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    THIRD_PARTY_NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    THIRD_PARTY_STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true),
                    THIRD_PARTY_STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    THIRD_PARTY_POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    THIRD_PARTY_COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    FORTHEBENEFITOF_PAYEE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FORTHEBENEFITOF_ACCOUNT_TYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TAX1099_FOR_EMPLOYEE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TAX1099_FOR_BENEFICIARY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FEDERAL_TAX_PERCENTAGE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAX_PERCENTAGE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    GROSS_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FEDERAL_TAX_AMOUNTE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAX_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    CHECK_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAXCODEID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DECEASED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GENDERID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    QDRO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MEMO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ROTH_IRA = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION", x => new { x.SSN, x.SEQUENCE_NUMBER });
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_COUNTRY_THIRD_PARTY_COUNTRY_ISO",
                        column: x => x.THIRD_PARTY_COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTION_FREQUENCY_FREQUENCYID",
                        column: x => x.FREQUENCY_ID,
                        principalTable: "DISTRIBUTION_FREQUENCY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTION_STATUS_STATUSID",
                        column: x => x.STATUS_ID,
                        principalTable: "DISTRIBUTION_STATUS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_GENDER_GENDERID",
                        column: x => x.GENDERID,
                        principalTable: "GENDER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_TAXCODE_TAXCODEID",
                        column: x => x.TAXCODEID,
                        principalTable: "TAXCODE",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BENEFICIARY_KIND",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "P", "Primary" },
                    { "S", "Secondary" }
                });

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "ID",
                value: (short)4);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "ID",
                value: (short)184);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "ID",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "ID",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "ID",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "ID",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "ID",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "ID",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "ID",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "ID",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "ID",
                value: (short)11);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "ID",
                value: (short)22);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "ID",
                value: (short)15);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "ID",
                value: (short)14);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "ID",
                value: (short)17);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "ID",
                value: (short)27);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "ID",
                value: (short)26);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "ID",
                value: (short)13);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "ID",
                value: (short)28);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "ID",
                value: (short)19);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "ID",
                value: (short)25);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "ID",
                value: (short)21);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "ID",
                value: (short)24);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "ID",
                value: (short)12);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "ID",
                value: (short)20);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "ID",
                value: (short)23);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "ID",
                value: (short)16);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "ID",
                value: (short)18);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "ID",
                value: (short)32);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "ID",
                value: (short)45);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "ID",
                value: (short)33);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "ID",
                value: (short)39);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "ID",
                value: (short)168);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "ID",
                value: (short)35);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "ID",
                value: (short)31);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "ID",
                value: (short)36);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "ID",
                value: (short)37);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "ID",
                value: (short)40);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "ID",
                value: (short)42);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "ID",
                value: (short)29);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "ID",
                value: (short)43);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "ID",
                value: (short)44);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "ID",
                value: (short)64);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "ID",
                value: (short)47);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "ID",
                value: (short)46);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "ID",
                value: (short)48);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "ID",
                value: (short)49);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "ID",
                value: (short)3);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "ID",
                value: (short)50);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "ID",
                value: (short)55);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "ID",
                value: (short)51);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "ID",
                value: (short)54);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "ID",
                value: (short)163);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "ID",
                value: (short)57);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "ID",
                value: (short)59);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "ID",
                value: (short)58);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "ID",
                value: (short)110);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "ID",
                value: (short)60);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "ID",
                value: (short)61);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "ID",
                value: (short)185);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "ID",
                value: (short)67);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "ID",
                value: (short)63);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "ID",
                value: (short)65);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "ID",
                value: (short)62);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "ID",
                value: (short)69);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "ID",
                value: (short)53);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "ID",
                value: (short)66);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "ID",
                value: (short)68);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "ID",
                value: (short)70);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "ID",
                value: (short)71);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "ID",
                value: (short)73);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "ID",
                value: (short)41);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "ID",
                value: (short)72);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "ID",
                value: (short)74);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "ID",
                value: (short)77);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "ID",
                value: (short)80);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "ID",
                value: (short)81);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "ID",
                value: (short)76);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "ID",
                value: (short)79);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "ID",
                value: (short)78);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "ID",
                value: (short)75);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "ID",
                value: (short)82);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "ID",
                value: (short)83);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "ID",
                value: (short)85);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "ID",
                value: (short)84);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "ID",
                value: (short)87);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "ID",
                value: (short)90);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "ID",
                value: (short)30);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "ID",
                value: (short)88);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "ID",
                value: (short)38);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "ID",
                value: (short)144);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "ID",
                value: (short)126);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "ID",
                value: (short)161);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "ID",
                value: (short)89);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "ID",
                value: (short)86);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "ID",
                value: (short)91);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "ID",
                value: (short)93);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "ID",
                value: (short)145);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "ID",
                value: (short)97);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "ID",
                value: (short)164);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "ID",
                value: (short)95);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "ID",
                value: (short)94);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "ID",
                value: (short)98);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "ID",
                value: (short)99);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "ID",
                value: (short)92);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "ID",
                value: (short)96);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "ID",
                value: (short)115);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "ID",
                value: (short)112);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "ID",
                value: (short)111);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "ID",
                value: (short)114);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "ID",
                value: (short)100);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "ID",
                value: (short)106);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "ID",
                value: (short)127);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "ID",
                value: (short)104);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "ID",
                value: (short)117);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "ID",
                value: (short)113);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "ID",
                value: (short)107);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "ID",
                value: (short)105);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "ID",
                value: (short)108);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "ID",
                value: (short)103);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "ID",
                value: (short)101);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "ID",
                value: (short)109);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "ID",
                value: (short)102);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "ID",
                value: (short)116);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "ID",
                value: (short)118);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "ID",
                value: (short)124);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "ID",
                value: (short)125);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "ID",
                value: (short)123);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "ID",
                value: (short)121);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "ID",
                value: (short)128);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "ID",
                value: (short)120);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "ID",
                value: (short)119);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "ID",
                value: (short)122);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "ID",
                value: (short)129);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "ID",
                value: (short)133);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "ID",
                value: (short)136);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "ID",
                value: (short)134);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "ID",
                value: (short)137);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "ID",
                value: (short)130);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "ID",
                value: (short)138);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "ID",
                value: (short)132);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "ID",
                value: (short)139);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "ID",
                value: (short)131);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "ID",
                value: (short)135);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "ID",
                value: (short)140);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "ID",
                value: (short)141);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "ID",
                value: (short)152);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "ID",
                value: (short)142);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "ID",
                value: (short)143);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "ID",
                value: (short)150);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "ID",
                value: (short)158);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "ID",
                value: (short)153);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "ID",
                value: (short)165);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "ID",
                value: (short)167);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "ID",
                value: (short)155);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "ID",
                value: (short)157);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "ID",
                value: (short)156);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "ID",
                value: (short)154);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "ID",
                value: (short)148);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "ID",
                value: (short)151);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "ID",
                value: (short)159);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "ID",
                value: (short)166);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "ID",
                value: (short)162);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "ID",
                value: (short)149);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "ID",
                value: (short)52);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "ID",
                value: (short)169);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "ID",
                value: (short)56);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "ID",
                value: (short)34);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "ID",
                value: (short)175);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "ID",
                value: (short)173);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "ID",
                value: (short)171);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "ID",
                value: (short)174);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "ID",
                value: (short)180);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "ID",
                value: (short)178);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "ID",
                value: (short)176);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "ID",
                value: (short)179);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "ID",
                value: (short)177);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "ID",
                value: (short)181);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "ID",
                value: (short)170);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "ID",
                value: (short)172);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "ID",
                value: (short)183);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "ID",
                value: (short)182);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "US",
                column: "ID",
                value: (short)186);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "ID",
                value: (short)187);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "ID",
                value: (short)188);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "ID",
                value: (short)146);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "ID",
                value: (short)190);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "ID",
                value: (short)191);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "ID",
                value: (short)189);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "ID",
                value: (short)147);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "ID",
                value: (short)192);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "ID",
                value: (short)160);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "ID",
                value: (short)193);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "ID",
                value: (short)194);

            migrationBuilder.InsertData(
                table: "DISTRIBUTION_FREQUENCY",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "A", "Annually" },
                    { "H", "Hardship" },
                    { "M", "Monthly" },
                    { "P", "Pay Direct" },
                    { "Q", "Quarterly" },
                    { "R", "Rollover Direct" }
                });

            migrationBuilder.InsertData(
                table: "DISTRIBUTION_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "C", "Manual Check" },
                    { "D", "Purge this Record" },
                    { "H", "Request is on hold" },
                    { "O", "Override vested amount in check (death or > 64 and 5 years vested" },
                    { "P", "Payment as been made" },
                    { "X", "Purge all records for the SSN" },
                    { "Y", "Request is OK to pay" },
                    { "Z", "Purge all records for the SSN" }
                });

            migrationBuilder.InsertData(
                table: "TAXCODE",
                columns: new[] { "CODE", "DESCRIPTION" },
                values: new object[,]
                {
                    { "1", "Early (Premature) dist no known exception" },
                    { "2", "Early (Premature) dist exception applies" },
                    { "3", "Disability" },
                    { "4", "Death" },
                    { "5", "Prohibited transaction" },
                    { "6", "Section 1035 exchange" },
                    { "7", "Normal distribution" },
                    { "8", "Excess contributions + earnings/deferrals" },
                    { "9", "PS 58 cost" },
                    { "A", "Qualifies for 5- or 10-year averaging" },
                    { "B", "Qualifies for death benefit exclusion" },
                    { "C", "Qualifies for both A and B" },
                    { "D", "Excess contributions + earnings deferrals" },
                    { "E", "Excess annual additions under section 415" },
                    { "F", "Charitable gift annuity" },
                    { "G", "Direct rollover to IRA" },
                    { "H", "Direct rollover to plan/tax sheltered annuity" },
                    { "P", "Excess contributions + earnings/deferrals" }
                });

            migrationBuilder.InsertData(
                table: "TERMINATIONCODE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "L", "Move Out of Area" },
                    { "M", "Poor Performance" },
                    { "N", "Off For Summer" },
                    { "O", "Workmans Compensation" },
                    { "P", "Injured" },
                    { "Q", "Transferred" },
                    { "R", "Retired" },
                    { "S", "Competition" },
                    { "T", "Another Job" },
                    { "U", "Would Not Rehire" },
                    { "V", "Never Reported" },
                    { "W", "Retired Receiving Pension" },
                    { "Y", "FMLA Approved" },
                    { "Z", "Deceased" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_KIND_ID",
                table: "BENEFICIARY",
                column: "KIND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_REL_PERCENT_KINDID",
                table: "BENEFICIARY_REL_PERCENT",
                column: "KIND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_REL_PERCENT_SSN",
                table: "BENEFICIARY_REL_PERCENT",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_COUNTRY_ISO",
                table: "DISTRIBUTION",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_FREQUENCYID",
                table: "DISTRIBUTION",
                column: "FREQUENCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_GENDERID",
                table: "DISTRIBUTION",
                column: "GENDERID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_TAXCODEID",
                table: "DISTRIBUTION",
                column: "TAXCODEID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_THIRD_PARTY_COUNTRY_ISO",
                table: "DISTRIBUTION",
                column: "THIRD_PARTY_COUNTRY_ISO");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARY_KIND_KIND_ID",
                table: "BENEFICIARY",
                column: "KIND_ID",
                principalTable: "BENEFICIARY_KIND",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODE_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                column: "PROFIT_CODE_ID",
                principalTable: "PROFITCODE",
                principalColumn: "CODE",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_DETAIL_TAXCODE_TAX_CODE_ID",
                table: "PROFIT_DETAIL",
                column: "TAX_CODE_ID",
                principalTable: "TAXCODE",
                principalColumn: "CODE",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARY_KIND_KIND_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODE_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_DETAIL_TAXCODE_TAX_CODE_ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_REL_PERCENT");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_KIND");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_FREQUENCY");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_STATUS");

            migrationBuilder.DropIndex(
                name: "IX_BENEFICIARY_KIND_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PROFIT_DETAIL",
                table: "PROFIT_DETAIL");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "6");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "7");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "8");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "9");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "A");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "B");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "C");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "D");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "E");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "F");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "G");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "H");

            migrationBuilder.DeleteData(
                table: "TAXCODE",
                keyColumn: "CODE",
                keyValue: "P");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "L");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "M");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "N");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "O");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "P");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "Q");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "R");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "S");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "T");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "U");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "V");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "W");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "Y");

            migrationBuilder.DeleteData(
                table: "TERMINATIONCODE",
                keyColumn: "ID",
                keyValue: "Z");

            migrationBuilder.DropColumn(
                name: "AMOUNT",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "DISTRIBUTION",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "EARNINGS",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "KIND_ID",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "SECONDARY_EARNINGS",
                table: "BENEFICIARY");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "MONTH_TO_DATE",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "REMARK",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropColumn(
                name: "YEAR_TO_DATE",
                table: "PROFIT_DETAIL");

            migrationBuilder.RenameTable(
                name: "PROFIT_DETAIL",
                newName: "PROFITDETAIL");

            migrationBuilder.RenameColumn(
                name: "DISTRIBUTION_SEQUENCE",
                table: "PROFITDETAIL",
                newName: "PROF_DIST_ID");

            migrationBuilder.RenameIndex(
                name: "IX_PROFIT_DETAIL_TAX_CODE_ID",
                table: "PROFITDETAIL",
                newName: "IX_PROFITDETAIL_TAX_CODE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_PROFIT_DETAIL_PROFIT_CODE_ID",
                table: "PROFITDETAIL",
                newName: "IX_PROFITDETAIL_PROFIT_CODE_ID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_MODIFIED_DATE",
                table: "DEMOGRAPHICS",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(2024, 8, 5, 14, 37, 1, 235, DateTimeKind.Local).AddTicks(1415),
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValue: new DateTime(2024, 8, 12, 17, 28, 33, 3, DateTimeKind.Local).AddTicks(3230));

            migrationBuilder.AddColumn<byte>(
                name: "BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<string>(
                name: "ZEROCONT",
                table: "PROFITDETAIL",
                type: "NVARCHAR2(1)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "COMMENT",
                table: "PROFITDETAIL",
                type: "NVARCHAR2(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "MONTH",
                table: "PROFITDETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<short>(
                name: "PROFIT_CLIENT",
                table: "PROFITDETAIL",
                type: "NUMBER(5)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "YEAR",
                table: "PROFITDETAIL",
                type: "NUMBER(5)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROFITDETAIL",
                table: "PROFITDETAIL",
                columns: new[] { "PROFIT_YEAR", "PROFIT_YEAR_ITERATION" });

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "ID",
                value: (byte)4);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "ID",
                value: (byte)184);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "ID",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "ID",
                value: (byte)6);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "ID",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "ID",
                value: (byte)8);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "ID",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "ID",
                value: (byte)7);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "ID",
                value: (byte)10);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "ID",
                value: (byte)9);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "ID",
                value: (byte)11);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "ID",
                value: (byte)22);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "ID",
                value: (byte)15);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "ID",
                value: (byte)14);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "ID",
                value: (byte)17);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "ID",
                value: (byte)27);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "ID",
                value: (byte)26);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "ID",
                value: (byte)13);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "ID",
                value: (byte)28);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "ID",
                value: (byte)19);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "ID",
                value: (byte)25);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "ID",
                value: (byte)21);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "ID",
                value: (byte)24);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "ID",
                value: (byte)12);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "ID",
                value: (byte)20);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "ID",
                value: (byte)23);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "ID",
                value: (byte)16);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "ID",
                value: (byte)18);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "ID",
                value: (byte)32);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "ID",
                value: (byte)45);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "ID",
                value: (byte)33);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "ID",
                value: (byte)39);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "ID",
                value: (byte)168);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "ID",
                value: (byte)35);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "ID",
                value: (byte)31);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "ID",
                value: (byte)36);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "ID",
                value: (byte)37);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "ID",
                value: (byte)40);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "ID",
                value: (byte)42);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "ID",
                value: (byte)29);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "ID",
                value: (byte)43);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "ID",
                value: (byte)44);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "ID",
                value: (byte)64);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "ID",
                value: (byte)47);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "ID",
                value: (byte)46);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "ID",
                value: (byte)48);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "ID",
                value: (byte)49);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "ID",
                value: (byte)3);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "ID",
                value: (byte)50);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "ID",
                value: (byte)55);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "ID",
                value: (byte)51);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "ID",
                value: (byte)54);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "ID",
                value: (byte)163);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "ID",
                value: (byte)57);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "ID",
                value: (byte)59);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "ID",
                value: (byte)58);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "ID",
                value: (byte)110);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "ID",
                value: (byte)60);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "ID",
                value: (byte)61);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "ID",
                value: (byte)185);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "ID",
                value: (byte)67);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "ID",
                value: (byte)63);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "ID",
                value: (byte)65);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "ID",
                value: (byte)62);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "ID",
                value: (byte)69);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "ID",
                value: (byte)53);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "ID",
                value: (byte)66);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "ID",
                value: (byte)68);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "ID",
                value: (byte)70);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "ID",
                value: (byte)71);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "ID",
                value: (byte)73);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "ID",
                value: (byte)41);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "ID",
                value: (byte)72);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "ID",
                value: (byte)74);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "ID",
                value: (byte)77);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "ID",
                value: (byte)80);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "ID",
                value: (byte)81);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "ID",
                value: (byte)76);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "ID",
                value: (byte)79);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "ID",
                value: (byte)78);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "ID",
                value: (byte)75);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "ID",
                value: (byte)82);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "ID",
                value: (byte)83);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "ID",
                value: (byte)85);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "ID",
                value: (byte)84);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "ID",
                value: (byte)87);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "ID",
                value: (byte)90);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "ID",
                value: (byte)30);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "ID",
                value: (byte)88);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "ID",
                value: (byte)38);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "ID",
                value: (byte)144);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "ID",
                value: (byte)126);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "ID",
                value: (byte)161);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "ID",
                value: (byte)89);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "ID",
                value: (byte)86);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "ID",
                value: (byte)91);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "ID",
                value: (byte)93);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "ID",
                value: (byte)145);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "ID",
                value: (byte)97);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "ID",
                value: (byte)164);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "ID",
                value: (byte)95);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "ID",
                value: (byte)94);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "ID",
                value: (byte)98);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "ID",
                value: (byte)99);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "ID",
                value: (byte)92);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "ID",
                value: (byte)96);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "ID",
                value: (byte)115);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "ID",
                value: (byte)112);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "ID",
                value: (byte)111);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "ID",
                value: (byte)114);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "ID",
                value: (byte)100);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "ID",
                value: (byte)106);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "ID",
                value: (byte)127);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "ID",
                value: (byte)104);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "ID",
                value: (byte)117);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "ID",
                value: (byte)113);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "ID",
                value: (byte)107);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "ID",
                value: (byte)105);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "ID",
                value: (byte)108);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "ID",
                value: (byte)103);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "ID",
                value: (byte)101);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "ID",
                value: (byte)109);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "ID",
                value: (byte)102);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "ID",
                value: (byte)116);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "ID",
                value: (byte)118);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "ID",
                value: (byte)124);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "ID",
                value: (byte)125);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "ID",
                value: (byte)123);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "ID",
                value: (byte)121);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "ID",
                value: (byte)128);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "ID",
                value: (byte)120);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "ID",
                value: (byte)119);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "ID",
                value: (byte)122);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "ID",
                value: (byte)129);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "ID",
                value: (byte)133);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "ID",
                value: (byte)136);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "ID",
                value: (byte)134);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "ID",
                value: (byte)137);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "ID",
                value: (byte)130);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "ID",
                value: (byte)138);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "ID",
                value: (byte)132);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "ID",
                value: (byte)139);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "ID",
                value: (byte)131);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "ID",
                value: (byte)135);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "ID",
                value: (byte)140);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "ID",
                value: (byte)141);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "ID",
                value: (byte)152);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "ID",
                value: (byte)142);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "ID",
                value: (byte)143);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "ID",
                value: (byte)150);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "ID",
                value: (byte)158);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "ID",
                value: (byte)153);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "ID",
                value: (byte)165);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "ID",
                value: (byte)167);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "ID",
                value: (byte)155);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "ID",
                value: (byte)157);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "ID",
                value: (byte)156);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "ID",
                value: (byte)154);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "ID",
                value: (byte)148);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "ID",
                value: (byte)151);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "ID",
                value: (byte)159);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "ID",
                value: (byte)166);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "ID",
                value: (byte)162);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "ID",
                value: (byte)149);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "ID",
                value: (byte)52);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "ID",
                value: (byte)169);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "ID",
                value: (byte)56);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "ID",
                value: (byte)34);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "ID",
                value: (byte)175);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "ID",
                value: (byte)173);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "ID",
                value: (byte)171);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "ID",
                value: (byte)174);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "ID",
                value: (byte)180);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "ID",
                value: (byte)178);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "ID",
                value: (byte)176);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "ID",
                value: (byte)179);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "ID",
                value: (byte)177);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "ID",
                value: (byte)181);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "ID",
                value: (byte)170);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "ID",
                value: (byte)172);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "ID",
                value: (byte)183);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "ID",
                value: (byte)182);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "US",
                column: "ID",
                value: (byte)186);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "ID",
                value: (byte)187);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "ID",
                value: (byte)188);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "ID",
                value: (byte)146);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "ID",
                value: (byte)190);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "ID",
                value: (byte)191);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "ID",
                value: (byte)189);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "ID",
                value: (byte)147);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "ID",
                value: (byte)192);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "ID",
                value: (byte)160);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "ID",
                value: (byte)193);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "ID",
                value: (byte)194);

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY",
                column: "BENEFICIARY_TYPE_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_BENEFICIARYTYPE_BENEFICIARY_TYPE_ID",
                table: "BENEFICIARY",
                column: "BENEFICIARY_TYPE_ID",
                principalTable: "BENEFICIARYTYPE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PROFITDETAIL_PROFITCODE_PROFIT_CODE_ID",
                table: "PROFITDETAIL",
                column: "PROFIT_CODE_ID",
                principalTable: "PROFITCODE",
                principalColumn: "CODE",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PROFITDETAIL_TAXCODE_TAX_CODE_ID",
                table: "PROFITDETAIL",
                column: "TAX_CODE_ID",
                principalTable: "TAXCODE",
                principalColumn: "CODE",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
