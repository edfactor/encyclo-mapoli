using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixBeneficiarySsnChangeHistory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_ID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "NUMBER(18)",
                precision: 18,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "NUMBER(18)",
                oldPrecision: 18)
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "BENEFICIARY_ID",
                principalTable: "BENEFICIARY_CONTACT",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                type: "NUMBER(18)",
                precision: 18,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(18)",
                oldPrecision: 18)
                .OldAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddForeignKey(
                name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_ID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "ID",
                principalTable: "BENEFICIARY_CONTACT",
                principalColumn: "ID");
        }
    }
}
