using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContactInfoAndAddressColumnsToHistory : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "CITY",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(25)",
                maxLength: 25,
                nullable: true,
                comment: "City from Address");

            migrationBuilder.AddColumn<string>(
                name: "EMAIL_ADDRESS",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true,
                comment: "EmailAddress from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "FIRST_NAME",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true,
                comment: "FirstName from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "LAST_NAME",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true,
                comment: "LastName from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "MIDDLE_NAME",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(25)",
                maxLength: 25,
                nullable: true,
                comment: "MiddleName from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "MOBILE_NUMBER",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(15)",
                maxLength: 15,
                nullable: true,
                comment: "MobileNumber from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "PHONE_NUMBER",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(15)",
                maxLength: 15,
                nullable: true,
                comment: "PhoneNumber from ContactInfo");

            migrationBuilder.AddColumn<string>(
                name: "POSTAL_CODE",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(9)",
                maxLength: 9,
                nullable: true,
                comment: "PostalCode from Address");

            migrationBuilder.AddColumn<string>(
                name: "STATE",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(3)",
                maxLength: 3,
                nullable: true,
                comment: "State from Address");

            migrationBuilder.AddColumn<string>(
                name: "STREET",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true,
                comment: "Street from Address");

            migrationBuilder.AddColumn<string>(
                name: "STREET2",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(30)",
                maxLength: 30,
                nullable: true,
                comment: "Street2 from Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CITY",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "EMAIL_ADDRESS",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "FIRST_NAME",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "LAST_NAME",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "MIDDLE_NAME",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "MOBILE_NUMBER",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "PHONE_NUMBER",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "POSTAL_CODE",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "STATE",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "STREET",
                table: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropColumn(
                name: "STREET2",
                table: "DEMOGRAPHIC_HISTORY");

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
