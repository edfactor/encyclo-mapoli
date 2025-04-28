using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_CHANGE",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    COLUMN_NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ORIGINAL_VALUE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    NEW_VALUE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    CHANGE_DATE = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_CHANGE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_EVENT",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TABLE_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    OPERATION = table.Column<string>(type: "NVARCHAR2(12)", maxLength: 12, nullable: false),
                    PRIMARY_KEY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_EVENT", x => x.ID);
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

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_EVENT_TABLENAME",
                table: "AUDIT_EVENT",
                column: "TABLE_NAME");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_CHANGE__AUDIT_EVENT");

            migrationBuilder.DropTable(
                name: "AUDIT_CHANGE");

            migrationBuilder.DropTable(
                name: "AUDIT_EVENT");
        }
    }
}
