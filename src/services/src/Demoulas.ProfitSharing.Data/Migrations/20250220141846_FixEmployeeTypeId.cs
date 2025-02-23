using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYMENT_TYPE_ID",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(1)",
                maxLength: 1,
                nullable: false,
                comment: "EmploymentType",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2)",
                oldMaxLength: 2,
                oldComment: "EmploymentType");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYMENT_TYPE_ID",
                table: "DEMOGRAPHIC",
                type: "NVARCHAR2(1)",
                maxLength: 1,
                nullable: false,
                comment: "EmploymentType",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2)",
                oldMaxLength: 2,
                oldComment: "EmploymentType");

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_SSN_CHANGE_HISTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BENEFICIARY_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    OLD_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    NEW_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    CREATED_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_SSN_CHANGE_HISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_BENEFICIARYID",
                        column: x => x.BENEFICIARY_ID,
                        principalTable: "BENEFICIARY",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    OLD_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    NEW_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    CREATED_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_SSN_CHANGE_HISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_SSN_CHANGE_HISTORY_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FAKE_SSNS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAKE_SSNS", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "BENEFICIARY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN_CHANGE_HISTORY_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                column: "DEMOGRAPHIC_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_SSN_CHANGE_HISTORY");

            migrationBuilder.DropTable(
                name: "FAKE_SSNS");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYMENT_TYPE_ID",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(2)",
                maxLength: 2,
                nullable: false,
                comment: "EmploymentType",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1)",
                oldMaxLength: 1,
                oldComment: "EmploymentType");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYMENT_TYPE_ID",
                table: "DEMOGRAPHIC",
                type: "NVARCHAR2(2)",
                maxLength: 2,
                nullable: false,
                comment: "EmploymentType",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1)",
                oldMaxLength: 1,
                oldComment: "EmploymentType");
        }
    }
}
