using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeSsnFromFrozenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SSN",
                table: "DEMOGRAPHIC_HISTORY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SSN",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NUMBER(9)",
                precision: 9,
                nullable: false,
                defaultValue: 0);
        }
    }
}
