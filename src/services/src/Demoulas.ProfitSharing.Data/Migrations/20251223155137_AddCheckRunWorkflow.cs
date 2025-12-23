using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckRunWorkflow : Migration
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
                name: "CHECKRUNWORKFLOWS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PROFITYEAR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CHECKRUNDATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    STEPNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STEPSTATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CHECKNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    REPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MAXREPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CREATEDBYUSERID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    MODIFIEDBYUSERID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    MODIFIEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHECKRUNWORKFLOWS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHECKRUNWORKFLOWS");

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
