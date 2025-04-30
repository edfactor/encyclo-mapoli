using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissiveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MISSIVES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MISSIVES", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "MISSIVES",
                columns: new[] { "ID", "MESSAGE" },
                values: new object[] { 1, "** VESTING INCREASED ON   CURRENT BALANCE ( > 1000 HRS) **" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MISSIVES");
        }
    }
}
