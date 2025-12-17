using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameComputedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            // PS-1829: Add computed FULL_NAME columns using raw SQL to avoid EF Core Unicode prefix issues
            // Oracle forces Unicode literals - use NVARCHAR2 to match AL16UTF16 character set

            // DEMOGRAPHIC.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE DEMOGRAPHIC DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE DEMOGRAPHIC ADD (
                    FULL_NAME NVARCHAR2(128) 
                    AS (LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");

            // BENEFICIARY_CONTACT_ARCHIVE.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE BENEFICIARY_CONTACT_ARCHIVE DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE BENEFICIARY_CONTACT_ARCHIVE ADD (
                    FULL_NAME NVARCHAR2(128) 
                    AS (LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");

            // BENEFICIARY_CONTACT.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE BENEFICIARY_CONTACT DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE BENEFICIARY_CONTACT ADD (
                    FULL_NAME NVARCHAR2(128) 
                    AS (LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);

            // PS-1829: Revert computed FULL_NAME columns back to VARCHAR2

            // DEMOGRAPHIC.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE DEMOGRAPHIC DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE DEMOGRAPHIC ADD (
                    FULL_NAME VARCHAR2(128) 
                    AS (LAST_NAME || q'[, ]' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN q'[ ]' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");

            // BENEFICIARY_CONTACT_ARCHIVE.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE BENEFICIARY_CONTACT_ARCHIVE DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE BENEFICIARY_CONTACT_ARCHIVE ADD (
                    FULL_NAME VARCHAR2(128) 
                    AS (LAST_NAME || q'[, ]' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN q'[ ]' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");

            // BENEFICIARY_CONTACT.FULL_NAME
            migrationBuilder.Sql(@"
                BEGIN
                    EXECUTE IMMEDIATE 'ALTER TABLE BENEFICIARY_CONTACT DROP COLUMN FULL_NAME';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -904 THEN -- ORA-00904: invalid identifier (column doesn't exist)
                            RAISE;
                        END IF;
                END;");

            migrationBuilder.Sql(@"
                ALTER TABLE BENEFICIARY_CONTACT ADD (
                    FULL_NAME VARCHAR2(128) 
                    AS (LAST_NAME || q'[, ]' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN q'[ ]' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END)
                )");
        }
    }
}
