using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDemographicHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_HISTORY",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    VALID_FROM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    VALID_TO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    EMPLOYEE_ID = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    STORE_NUMBER = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false, comment: "StoreNumber"),
                    PAY_CLASSIFICATION_ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false, comment: "PayClassification"),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    HIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    REHIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TERMINATION_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    DEPARTMENT = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    EMPLOYMENT_TYPE_ID = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, comment: "EmploymentType"),
                    PAY_FREQUENCY_ID = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    TERMINATION_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EMPLOYMENT_STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    CREATED_DATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_HISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_HISTORY_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_EMPLOYEEID",
                table: "DEMOGRAPHIC_HISTORY",
                column: "EMPLOYEE_ID");

            migrationBuilder.Sql(@"
                INSERT INTO DEMOGRAPHIC_HISTORY(DEMOGRAPHIC_ID, VALID_FROM, VALID_TO, ORACLE_HCM_ID, SSN, EMPLOYEE_ID, STORE_NUMBER, PAY_CLASSIFICATION_ID, DATE_OF_BIRTH, HIRE_DATE, REHIRE_DATE, TERMINATION_DATE, DEPARTMENT, EMPLOYMENT_TYPE_ID, PAY_FREQUENCY_ID, TERMINATION_CODE_ID, EMPLOYMENT_STATUS_ID, CREATED_DATETIME)
                SELECT ID, TO_TIMESTAMP('01-01-1900 00:00:00','MM-DD-YYYY HH24:MI:SS'), TO_TIMESTAMP('01-01-2100 00:00:00','MM-DD-YYYY HH24:MI:SS'), ORACLE_HCM_ID, SSN, EMPLOYEE_ID, STORE_NUMBER, PAY_CLASSIFICATION_ID, DATE_OF_BIRTH, HIRE_DATE, REHIRE_DATE, TERMINATION_DATE, DEPARTMENT, EMPLOYMENT_TYPE_ID, PAY_FREQUENCY_ID, TERMINATION_CODE_ID, EMPLOYMENT_STATUS_ID, sys_extract_utc(systimestamp)
                  FROM DEMOGRAPHIC;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_HISTORY");
        }
    }
}
