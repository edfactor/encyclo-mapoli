using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addAnnuityRateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ANNUITY_RATE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    SINGLE_RATE = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: false),
                    JOINT_RATE = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNUITY_RATE", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ANNUITY_RATE_YEAR_AGE",
                table: "ANNUITY_RATE",
                columns: new[] { "YEAR", "AGE" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ANNUITY_RATE");
        }
    }
}
