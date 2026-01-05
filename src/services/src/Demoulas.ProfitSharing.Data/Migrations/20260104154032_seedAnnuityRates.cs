using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedAnnuityRates : Migration
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
                name: "ANNUITY_RATE_CONFIG",
                columns: table => new
                {
                    YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    MINIMUM_AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    MAXIMUM_AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNUITY_RATE_CONFIG", x => x.YEAR);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ANNUITY_RATE_CONFIG");

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
