using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addBeneficiaryContactArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BENEFICIARY_CONTACT_ARCHIVE",
                columns: table => new
                {
                    ARCHIVE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false, comment: "FullName"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE"),
                    DELETE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DELETED_BY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_CONTACT_ARCHIVE", x => x.ARCHIVE_ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_CONTACT_ARCHIVE_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_CONTACT_ARCHIVE_COUNTRY_ISO",
                table: "BENEFICIARY_CONTACT_ARCHIVE",
                column: "COUNTRY_ISO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BENEFICIARY_CONTACT_ARCHIVE");
        }
    }
}
