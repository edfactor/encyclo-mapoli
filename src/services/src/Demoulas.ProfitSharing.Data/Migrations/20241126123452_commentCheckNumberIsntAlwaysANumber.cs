using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class commentCheckNumberIsntAlwaysANumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE PROFIT_DETAIL SET COMMENT_RELATED_CHECK_NUMBER=null");


            migrationBuilder.AlterColumn<string>(
                name: "COMMENT_RELATED_CHECK_NUMBER",
                table: "PROFIT_DETAIL",
                type: "NVARCHAR2(9)",
                maxLength: 9,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("UPDATE PROFIT_DETAIL SET COMMENT_RELATED_CHECK_NUMBER=null");
            migrationBuilder.AlterColumn<int>(
                name: "COMMENT_RELATED_CHECK_NUMBER",
                table: "PROFIT_DETAIL",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(9)",
                oldMaxLength: 9,
                oldNullable: true);
        }
    }
}
