using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class YearEndUpdateStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YE_UPDATE_STATUS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    UPDATED_DATE = table.Column<DateTime>(type: "DATE", nullable: false),
                    UPDATED_BY = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    BENEFICIARIES_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    EMPLOYEES_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    ETVAS_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    CONTRIBUTION_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    INCOMING_FORFEIT_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    EARNINGS_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    SECONDARY_EARNINGS_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    MAX_ALLOWED_CONTRIBUTIONS = table.Column<long>(type: "NUMBER(6)", precision: 6, nullable: false),
                    BADGE_ADJUSTED = table.Column<long>(type: "NUMBER(7)", precision: 7, nullable: false),
                    BADGE_ADJUSTED2 = table.Column<long>(type: "NUMBER(7)", precision: 7, nullable: false),
                    ADJUST_CONTRIBUTION_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_EARNINGS_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_INCOMING_FORFEIT_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_EARNINGS_SECONDARY_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YE_UPDATE_STATUS", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YE_UPDATE_STATUS_PROFIT_YEAR",
                table: "YE_UPDATE_STATUS",
                column: "PROFIT_YEAR",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YE_UPDATE_STATUS");
        }
    }
}
