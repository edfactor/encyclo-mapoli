using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFtpOperationLog : Migration
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
                name: "FTPOPERATIONLOGS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CHECKRUNWORKFLOWID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OPERATIONTYPE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FILENAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DESTINATION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ISSUCCESS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ERRORMESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DURATIONMS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    USERID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTPOPERATIONLOGS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FTPOPERATIONLOGS");

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
