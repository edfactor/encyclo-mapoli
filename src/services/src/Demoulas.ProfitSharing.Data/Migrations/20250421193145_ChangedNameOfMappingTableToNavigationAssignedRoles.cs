using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNameOfMappingTableToNavigationAssignedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATIONROLES_REQUIREDROLESID",
                table: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NAVIGATIONNAVIGATIONROLE",
                table: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.RenameTable(
                name: "NAVIGATIONNAVIGATIONROLE",
                newName: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "NAVIGATION_ROLE",
                newName: "NAME");

            migrationBuilder.RenameIndex(
                name: "IX_NAVIGATIONNAVIGATIONROLE_REQUIREDROLESID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                newName: "IX_NAVIGATION_ASSIGNED_ROLES_REQUIREDROLESID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NAVIGATION_ASSIGNED_ROLES",
                table: "NAVIGATION_ASSIGNED_ROLES",
                columns: new[] { "NAVIGATIONID", "REQUIREDROLESID" });

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATIONROLES_REQUIREDROLESID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                column: "REQUIREDROLESID",
                principalTable: "NAVIGATION_ROLE",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                column: "NAVIGATIONID",
                principalTable: "NAVIGATION",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATIONROLES_REQUIREDROLESID",
                table: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.DropForeignKey(
                name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NAVIGATION_ASSIGNED_ROLES",
                table: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.RenameTable(
                name: "NAVIGATION_ASSIGNED_ROLES",
                newName: "NAVIGATIONNAVIGATIONROLE");

            migrationBuilder.RenameColumn(
                name: "NAME",
                table: "NAVIGATION_ROLE",
                newName: "URL");

            migrationBuilder.RenameIndex(
                name: "IX_NAVIGATION_ASSIGNED_ROLES_REQUIREDROLESID",
                table: "NAVIGATIONNAVIGATIONROLE",
                newName: "IX_NAVIGATIONNAVIGATIONROLE_REQUIREDROLESID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NAVIGATIONNAVIGATIONROLE",
                table: "NAVIGATIONNAVIGATIONROLE",
                columns: new[] { "NAVIGATIONID", "REQUIREDROLESID" });

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATIONROLES_REQUIREDROLESID",
                table: "NAVIGATIONNAVIGATIONROLE",
                column: "REQUIREDROLESID",
                principalTable: "NAVIGATION_ROLE",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NAVIGATIONNAVIGATIONROLE_NAVIGATION_NAVIGATIONID",
                table: "NAVIGATIONNAVIGATIONROLE",
                column: "NAVIGATIONID",
                principalTable: "NAVIGATION",
                principalColumn: "ID");
        }
    }
}
