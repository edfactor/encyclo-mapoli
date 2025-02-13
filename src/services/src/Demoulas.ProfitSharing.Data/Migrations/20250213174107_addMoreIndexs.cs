using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addMoreIndexs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC",
                column: "BADGE_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN_BADGENUMBER",
                table: "DEMOGRAPHIC",
                columns: new[] { "SSN", "BADGE_NUMBER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_SSN_BADGENUMBER",
                table: "DEMOGRAPHIC");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC",
                column: "BADGE_NUMBER");
        }
    }
}
