using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class trackChecks : Migration
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
                name: "CHECK_RUN_WORKFLOW",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PROFITYEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    CHECKRUNDATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    STEPNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STEPSTATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CHECKNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    REPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    MAXREPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 2),
                    CREATEDBYUSERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CREATEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    MODIFIEDBYUSERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    MODIFIEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHECK_RUN_WORKFLOW", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FILE_TRANSFER_AUDIT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    FILENAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DESTINATION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    FILESIZE = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TRANSFERDURATIONMS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ISSUCCESS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ERRORMESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CSVCONTENT = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    CHECKRUNWORKFLOWID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_TRANSFER_AUDIT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FTP_OPERATION_LOG",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CHECKRUNWORKFLOWID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OPERATIONTYPE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FILENAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DESTINATION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ISSUCCESS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ERRORMESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    DURATIONMS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTP_OPERATION_LOG", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHECK_RUN_WORKFLOW_PROFIT_YEAR",
                table: "CHECK_RUN_WORKFLOW",
                column: "PROFITYEAR");

            migrationBuilder.CreateIndex(
                name: "IX_CHECK_RUN_WORKFLOW_RUN_DATE",
                table: "CHECK_RUN_WORKFLOW",
                column: "CHECKRUNDATE");

            migrationBuilder.CreateIndex(
                name: "IX_CHECK_RUN_WORKFLOW_YEAR_STATUS",
                table: "CHECK_RUN_WORKFLOW",
                columns: new[] { "PROFITYEAR", "STEPSTATUS" });

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TRANSFER_AUDIT_FILENAME",
                table: "FILE_TRANSFER_AUDIT",
                column: "FILENAME");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TRANSFER_AUDIT_TIMESTAMP",
                table: "FILE_TRANSFER_AUDIT",
                column: "TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TRANSFER_AUDIT_WORKFLOW_ID",
                table: "FILE_TRANSFER_AUDIT",
                column: "CHECKRUNWORKFLOWID");

            migrationBuilder.CreateIndex(
                name: "IX_FTP_OPERATION_LOG_TIMESTAMP",
                table: "FTP_OPERATION_LOG",
                column: "TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_FTP_OPERATION_LOG_WORKFLOW_ID",
                table: "FTP_OPERATION_LOG",
                column: "CHECKRUNWORKFLOWID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHECK_RUN_WORKFLOW");

            migrationBuilder.DropTable(
                name: "FILE_TRANSFER_AUDIT");

            migrationBuilder.DropTable(
                name: "FTP_OPERATION_LOG");

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
