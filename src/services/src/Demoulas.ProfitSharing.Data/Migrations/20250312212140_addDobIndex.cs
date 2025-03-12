using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDobIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_DATEOFBIRTH",
                table: "DEMOGRAPHIC",
                column: "DATE_OF_BIRTH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_DATEOFBIRTH",
                table: "DEMOGRAPHIC");
        }
    }
}
