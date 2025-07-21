using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addExcludedIdTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EXCLUDED_ID_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXCLUDED_ID_TYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EXCLUDED_ID",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EXCLUDED_ID_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EXCLUDED_ID_VALUE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXCLUDED_ID", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EXCLUDED_ID_EXCLUDEDIDTYPE_EXCLUDEDIDTYPEID",
                        column: x => x.EXCLUDED_ID_TYPE_ID,
                        principalTable: "EXCLUDED_ID_TYPE",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "EXCLUDED_ID_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "QPay066TA Exclusions" },
                    { (byte)2, "QPay066I Exclusions" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EXCLUDED_ID_EXCLUDEDIDTYPEID",
                table: "EXCLUDED_ID",
                column: "EXCLUDED_ID_TYPE_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EXCLUDED_ID");

            migrationBuilder.DropTable(
                name: "EXCLUDED_ID_TYPE");
        }
    }
}
