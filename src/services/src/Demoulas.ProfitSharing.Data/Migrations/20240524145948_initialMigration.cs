using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Definition",
                columns: table => new
                {
                    Key = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definition", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHICS",
                columns: table => new
                {
                    DEM_BADGE = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    PY_NAM = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    PY_LNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    PY_FNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    PY_MNAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true),
                    PY_STOR = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false),
                    PY_DP = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false),
                    PY_CLA = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false),
                    PY_ADD = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    PY_ADD2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    PY_CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false),
                    PY_STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false),
                    PY_ZIP = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PY_DOB = table.Column<int>(type: "NUMBER(8)", precision: 8, nullable: false),
                    PY_FULL_DT = table.Column<int>(type: "NUMBER(8)", precision: 8, nullable: false),
                    PY_HIRE_DT = table.Column<int>(type: "NUMBER(8)", precision: 8, nullable: false),
                    PY_REHIRE_DT = table.Column<int>(type: "NUMBER(8)", precision: 8, nullable: false),
                    PY_TERM_DT = table.Column<int>(type: "NUMBER(8)", precision: 8, nullable: false),
                    PY_FUL = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    PY_FREQ = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHICS", x => x.DEM_BADGE);
                });

            migrationBuilder.InsertData(
                table: "Definition",
                columns: new[] { "Key", "Description" },
                values: new object[,]
                {
                    { "F", "Full time 8 paid holidays " },
                    { "G", "Full time accrued paid holidays" },
                    { "H", "Full time(straight salary)" },
                    { "P", "Part Time" },
                    { "PF", "Pay frequency (1=weekly, 2=monthly)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Definition");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHICS");
        }
    }
}
