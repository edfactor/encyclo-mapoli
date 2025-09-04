using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixBeneficiarySsnChangeHistory3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.RenameIndex(
                name: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                newName: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYCONTACTID");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYCONTACTID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "BENEFICIARY_ID",
                principalTable: "BENEFICIARY_CONTACT",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYCONTACTID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.RenameIndex(
                name: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYCONTACTID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                newName: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYID");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "BENEFICIARY_ID",
                principalTable: "BENEFICIARY_CONTACT",
                principalColumn: "ID");
        }
    }
}
