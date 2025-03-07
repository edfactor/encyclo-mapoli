using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class dupNameSsnIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPE_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_FULL_NAME",
                table: "DEMOGRAPHIC",
                column: "FULL_NAME");

            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPES_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_TYPE_ID",
                principalTable: "EMPLOYMENT_TYPE",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPES_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropIndex(
                name: "IX_DEMOGRAPHIC_FULL_NAME",
                table: "DEMOGRAPHIC");

            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPE_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_TYPE_ID",
                principalTable: "EMPLOYMENT_TYPE",
                principalColumn: "ID");
        }
    }
}
