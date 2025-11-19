using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addStates : Migration
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

            migrationBuilder.CreateTable(
                name: "STATE",
                columns: table => new
                {
                    ABBREVIATION = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STATE", x => x.ABBREVIATION);
                });

            migrationBuilder.InsertData(
                table: "STATE",
                columns: new[] { "ABBREVIATION", "NAME" },
                values: new object[,]
                {
                    { "AK", "Alaska" },
                    { "AL", "Alabama" },
                    { "AR", "Arkansas" },
                    { "AS", "American Samoa" },
                    { "AZ", "Arizona" },
                    { "CA", "California" },
                    { "CO", "Colorado" },
                    { "CT", "Connecticut" },
                    { "DC", "District of Columbia" },
                    { "DE", "Delaware" },
                    { "FL", "Florida" },
                    { "GA", "Georgia" },
                    { "GU", "Guam" },
                    { "HI", "Hawaii" },
                    { "IA", "Iowa" },
                    { "ID", "Idaho" },
                    { "IL", "Illinois" },
                    { "IN", "Indiana" },
                    { "KS", "Kansas" },
                    { "KY", "Kentucky" },
                    { "LA", "Louisiana" },
                    { "MA", "Massachusetts" },
                    { "MD", "Maryland" },
                    { "ME", "Maine" },
                    { "MI", "Michigan" },
                    { "MN", "Minnesota" },
                    { "MO", "Missouri" },
                    { "MP", "Northern Mariana Islands" },
                    { "MS", "Mississippi" },
                    { "MT", "Montana" },
                    { "NC", "North Carolina" },
                    { "ND", "North Dakota" },
                    { "NE", "Nebraska" },
                    { "NH", "New Hampshire" },
                    { "NJ", "New Jersey" },
                    { "NM", "New Mexico" },
                    { "NV", "Nevada" },
                    { "NY", "New York" },
                    { "OH", "Ohio" },
                    { "OK", "Oklahoma" },
                    { "OR", "Oregon" },
                    { "PA", "Pennsylvania" },
                    { "PR", "Puerto Rico" },
                    { "RI", "Rhode Island" },
                    { "SC", "South Carolina" },
                    { "SD", "South Dakota" },
                    { "TN", "Tennessee" },
                    { "TX", "Texas" },
                    { "UM", "United States Minor Outlying Islands" },
                    { "UT", "Utah" },
                    { "VA", "Virginia" },
                    { "VI", "Virgin Islands" },
                    { "VT", "Vermont" },
                    { "WA", "Washington" },
                    { "WI", "Wisconsin" },
                    { "WV", "West Virginia" },
                    { "WY", "Wyoming" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STATE");

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
