using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addForfeitCaCommentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "COMMENT_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)25, "Forfeit Class Action" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25);
        }
    }
}
