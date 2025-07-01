using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addBeneficiaryArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BENEFICIARY_ARCHIVE",
                columns: table => new
                {
                    ARCHIVE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    PSN_SUFFIX = table.Column<short>(type: "NUMBER(5)", precision: 5, nullable: false),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BENEFICIARY_CONTACT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    KIND_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    DELETE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DELETED_BY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_ARCHIVE", x => x.ARCHIVE_ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BENEFICIARY_ARCHIVE");
        }
    }
}
