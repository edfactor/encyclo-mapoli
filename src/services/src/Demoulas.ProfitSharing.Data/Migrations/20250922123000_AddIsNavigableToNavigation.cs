using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    public partial class AddIsNavigableToNavigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IS_NAVIGABLE column as NUMBER(1) with default 1 (true) to preserve current behavior
            migrationBuilder.AddColumn<bool>(
                name: "IS_NAVIGABLE",
                table: "NAVIGATION",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: true);

            // Optional: if existing rows should be set based on URL presence, that could be done here.
            // For safety we default to true to preserve current navigable items.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_NAVIGABLE",
                table: "NAVIGATION");
        }
    }
}
