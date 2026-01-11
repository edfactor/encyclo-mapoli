using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfitShareCheckNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DECLARE
    v_start NUMBER;
BEGIN
    BEGIN
        EXECUTE IMMEDIATE 'DROP SEQUENCE PROFIT_SHARE_CHECK_NUMBER_SEQ';
    EXCEPTION
        WHEN OTHERS THEN
            IF SQLCODE != -2289 THEN
                RAISE;
            END IF;
    END;

    SELECT NVL(MAX(CHECK_NUMBER), 0) + 1
    INTO v_start
    FROM PROFIT_SHARE_CHECK;

    EXECUTE IMMEDIATE 'CREATE SEQUENCE PROFIT_SHARE_CHECK_NUMBER_SEQ '
        || ' START WITH ' || v_start
        || ' INCREMENT BY 1 '
        || ' MINVALUE 1 '
        || ' NOCYCLE '
        || ' NOCACHE';
END;",
                suppressTransaction: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"BEGIN
    EXECUTE IMMEDIATE 'DROP SEQUENCE PROFIT_SHARE_CHECK_NUMBER_SEQ';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -2289 THEN
            RAISE;
        END IF;
END;",
                suppressTransaction: false);
        }
    }
}
