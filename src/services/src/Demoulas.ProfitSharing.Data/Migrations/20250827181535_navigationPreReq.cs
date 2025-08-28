using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class navigationPreReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_PARENTID",
                table: "NAVIGATION");

            migrationBuilder.RenameIndex(
                name: "IX_NAVIGATION_PARENTID",
                table: "NAVIGATION",
                newName: "IX_NAVIGATION_PARENT_ID");

            migrationBuilder.CreateTable(
                name: "NAVIGATION_PREREQUISITES",
                columns: table => new
                {
                    NAVIGATION_ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PREREQUISITE_ID = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_PREREQUISITES", x => new { x.NAVIGATION_ID, x.PREREQUISITE_ID });
                    table.ForeignKey(
                        name: "FK_NAV_PREREQ_DEPENDENT",
                        column: x => x.NAVIGATION_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAV_PREREQ_PREREQUISITE",
                        column: x => x.PREREQUISITE_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_PREREQUISITES_PREREQUISITE_ID",
                table: "NAVIGATION_PREREQUISITES",
                column: "PREREQUISITE_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_PARENT_ID",
                table: "NAVIGATION",
                column: "PARENT_ID",
                principalTable: "NAVIGATION",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_PARENT_ID",
                table: "NAVIGATION");

            migrationBuilder.DropTable(
                name: "NAVIGATION_PREREQUISITES");

            migrationBuilder.RenameIndex(
                name: "IX_NAVIGATION_PARENT_ID",
                table: "NAVIGATION",
                newName: "IX_NAVIGATION_PARENTID");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_NAVIGATION_PARENTID",
                table: "NAVIGATION",
                column: "PARENT_ID",
                principalTable: "NAVIGATION",
                principalColumn: "ID");
        }
    }
}
