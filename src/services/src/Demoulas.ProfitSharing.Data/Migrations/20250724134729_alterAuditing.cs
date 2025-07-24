using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class alterAuditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_CHANGE__AUDIT_EVENT");

            migrationBuilder.DropTable(
                name: "AUDIT_CHANGE");

            migrationBuilder.AlterColumn<string>(
                name: "PRIMARY_KEY",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OPERATION",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(12)",
                oldMaxLength: 12);

            migrationBuilder.AddColumn<string>(
                name: "CHANGES_JSON",
                table: "AUDIT_EVENT",
                type: "CLOB",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT",
                table: "AUDIT_EVENT",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: false,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CHANGES_JSON",
                table: "AUDIT_EVENT");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "AUDIT_EVENT");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "AUDIT_EVENT");

            migrationBuilder.AlterColumn<string>(
                name: "PRIMARY_KEY",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OPERATION",
                table: "AUDIT_EVENT",
                type: "NVARCHAR2(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(24)",
                oldMaxLength: 24);

            migrationBuilder.CreateTable(
                name: "AUDIT_CHANGE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CHANGE_DATE = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    COLUMN_NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NEW_VALUE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    ORIGINAL_VALUE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_CHANGE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_CHANGE__AUDIT_EVENT",
                columns: table => new
                {
                    AUDITEVENTID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CHANGESID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_CHANGE__AUDIT_EVENT", x => new { x.AUDITEVENTID, x.CHANGESID });
                    table.ForeignKey(
                        name: "FK_AUDIT_CHANGE__AUDIT_EVENT_AUDITCHANGE_CHANGESID",
                        column: x => x.CHANGESID,
                        principalTable: "AUDIT_CHANGE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AUDIT_CHANGE__AUDIT_EVENT_AUDIT_EVENT_AUDITEVENTID",
                        column: x => x.AUDITEVENTID,
                        principalTable: "AUDIT_EVENT",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_CHANGE__AUDIT_EVENT_CHANGESID",
                table: "AUDIT_CHANGE__AUDIT_EVENT",
                column: "CHANGESID");
        }
    }
}
