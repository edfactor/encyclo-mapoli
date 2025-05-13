using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class auditEventPkSizeChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PRIMARY_KEY",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(24)",
                oldMaxLength: 24,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PRIMARY_KEY",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(24)",
                maxLength: 24,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
