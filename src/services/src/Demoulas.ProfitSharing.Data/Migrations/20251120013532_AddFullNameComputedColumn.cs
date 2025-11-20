using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameComputedColumn : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "FULL_NAME",
                table: "BENEFICIARY_CONTACT",
                type: "VARCHAR2(128)",
                maxLength: 128,
                nullable: true,
                computedColumnSql: "LAST_NAME || UNISTR(', ') || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN UNISTR(' ') || SUBSTR(MIDDLE_NAME,1,1) ELSE UNISTR('') END",
                stored: true,
                comment: "Automatically computed from LastName, FirstName, and MiddleName with middle initial",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(84)",
                oldMaxLength: 84,
                oldComment: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FULL_NAME",
                table: "BENEFICIARY_CONTACT",
                type: "VARCHAR2(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                comment: "FullName",
                oldClrType: typeof(string),
                oldType: "VARCHAR2(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComputedColumnSql: "LAST_NAME || UNISTR(', ') || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN UNISTR(' ') || SUBSTR(MIDDLE_NAME,1,1) ELSE UNISTR('') END",
                oldComment: "Automatically computed from LastName, FirstName, and MiddleName with middle initial");
        }
    }
}
