using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkProfitShareCheckToWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CHECK_RUN_WORKFLOW_ID",
                table: "PROFIT_SHARE_CHECK",
                type: "RAW(16)",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK",
                column: "CHECK_RUN_WORKFLOW_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_SHARE_CHECK_CHECK_RUN_WORKFLOW_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK",
                column: "CHECK_RUN_WORKFLOW_ID",
                principalTable: "CHECK_RUN_WORKFLOW",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_SHARE_CHECK_CHECK_RUN_WORKFLOW_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropIndex(
                name: "IX_PROFIT_SHARE_CHECK_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropColumn(
                name: "CHECK_RUN_WORKFLOW_ID",
                table: "PROFIT_SHARE_CHECK");

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
