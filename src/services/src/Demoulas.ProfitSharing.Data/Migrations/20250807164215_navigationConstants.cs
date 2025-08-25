using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class navigationConstants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "NAVIGATION",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)")
                .OldAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[,]
                {
                    { 50, false, "", (byte)1, null, (byte)1, "", "INQUIRIES", "" },
                    { 52, true, "", (byte)2, null, (byte)1, "", "BENEFICIARIES", "" },
                    { 53, true, "", (byte)3, null, (byte)1, "", "DISTRIBUTIONS", "" },
                    { 54, true, "", (byte)4, null, (byte)1, "", "RECONCILIATION", "" },
                    { 55, false, "", (byte)5, null, (byte)1, "", "YEAR END", "" },
                    { 56, false, "", (byte)6, null, (byte)1, "", "IT OPERATIONS", "" },
                    { 1, false, "", (byte)1, 55, (byte)1, "", "December Activities", "december-process-accordion" },
                    { 14, false, "", (byte)2, 55, (byte)1, "", "Fiscal Close", "fiscal-close" },
                    { 51, false, "", (byte)1, 50, (byte)1, "", "MASTER INQUIRY", "master-inquiry" },
                    { 57, false, "", (byte)1, 56, (byte)1, "", "Demographic Freeze", "demographic-freeze" },
                    { 2, false, "", (byte)1, 1, (byte)1, "", "Clean up Reports", "" },
                    { 7, false, "", (byte)3, 1, (byte)1, "008-13", "Military Contributions", "military-entry-and-modification" },
                    { 8, false, "", (byte)2, 1, (byte)1, "QPREV-PROF", "Unforfeit", "unforfeitures" },
                    { 9, false, "", (byte)4, 1, (byte)1, "QPAY066", "Terminations", "prof-term" },
                    { 10, false, "", (byte)5, 1, (byte)1, "008-12", "Forfeitures", "forfeitures-adjustment" },
                    { 11, false, "", (byte)6, 1, (byte)1, "QPAY129", "Distributions and Forfeitures", "distributions-and-forfeitures" },
                    { 13, false, "", (byte)9, 1, (byte)1, "PAY426", "Profit Share Report", "profit-share-report" },
                    { 15, false, "", (byte)1, 14, (byte)1, "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09", "Manage Executive Hours", "manage-executive-hours-and-dollars" },
                    { 16, false, "", (byte)2, 14, (byte)1, "PROF-DOLLAR-EXTRACT", "YTD Wages Extract", "ytd-wages-extract" },
                    { 17, false, "", (byte)4, 14, (byte)1, "PAY426", "Profit Share Report (Final Run)", "profit-share-report" },
                    { 18, false, "", (byte)3, 14, (byte)1, "PAY426", "Profit Share Report (Edit Run)", "pay426n" },
                    { 30, false, "", (byte)5, 14, (byte)1, "GET-ELIGIBLE-EMPS", "Get Eligible Employees", "eligible-employees" },
                    { 31, false, "", (byte)6, 14, (byte)1, "PAY443", "Profit Share Forfeit", "forfeit" },
                    { 33, false, "", (byte)10, 14, (byte)1, "PAY450", "Prof PayMaster Update", "pay450-summary" },
                    { 34, false, "", (byte)12, 14, (byte)1, "Prof130", "Prof Share Report By Age", "" },
                    { 41, false, "", (byte)13, 14, (byte)1, "QPAY501", "Prof Share Gross Rpt", "profit-share-gross-report" },
                    { 42, false, "", (byte)14, 14, (byte)1, "QPAY066TA", "Prof Share by Store", "" },
                    { 49, false, "", (byte)15, 14, (byte)1, "PAYCERT", "Print Profit Certs", "print-profit-certs" },
                    { 60, false, "", (byte)7, 14, (byte)1, "PAY444|PAY447", "Master Update", "profit-share-update" },
                    { 62, true, "", (byte)8, 14, (byte)1, "PAY460, PROFTLD", "Profit Master Update", "profit-master-update" },
                    { 63, false, "", (byte)16, 14, (byte)1, "", "Save Prof Paymstr", "save-prof-paymstr" },
                    { 64, false, "", (byte)11, 14, (byte)1, "PROF-CNTRL-SHEET", "Prof Control Sheet", "prof-control-sheet" },
                    { 65, false, "", (byte)17, 14, (byte)1, "QPAY066*", "QPAY066* Ad Hoc Reports", "qpay066-adhoc" },
                    { 3, false, "", (byte)1, 2, (byte)1, "", "Demographic Badges Not In PayProfit", "demographic-badges-not-in-payprofit" },
                    { 4, false, "", (byte)2, 2, (byte)1, "", "Duplicate SSNs in Demographics", "duplicate-ssns-demographics" },
                    { 5, false, "", (byte)3, 2, (byte)1, "", "Negative ETVA", "negative-etva-for-ssns-on-payprofit" },
                    { 6, false, "", (byte)4, 2, (byte)1, "", "Duplicate Names and Birthdays", "duplicate-names-and-birthdays" },
                    { 35, false, "", (byte)2, 34, (byte)1, "PROF130", "CONTRIBUTIONS BY AGE", "contributions-by-age" },
                    { 36, false, "", (byte)1, 34, (byte)1, "PROF130", "DISTRIBUTIONS BY AGE", "distributions-by-age" },
                    { 37, false, "", (byte)3, 34, (byte)1, "PROF130", "FORFEITURES BY AGE", "forfeitures-by-age" },
                    { 38, false, "", (byte)4, 34, (byte)1, "PROF130B", "BALANCE BY AGE", "balance-by-age" },
                    { 39, false, "", (byte)5, 34, (byte)1, "PROF130V", "VESTED AMOUNTS BY AGE", "vested-amounts-by-age" },
                    { 40, false, "", (byte)6, 34, (byte)1, "PROF130Y", "BALANCE BY YEARS", "balance-by-years" },
                    { 43, false, "", (byte)1, 42, (byte)1, "", "QPAY066-UNDR21", "qpay066-under21" },
                    { 44, false, "", (byte)2, 42, (byte)1, "", "QPAY066TA-UNDR21", "qpay066ta-under21" },
                    { 45, false, "", (byte)3, 42, (byte)1, "", "QPAY066TA", "qpay066ta" },
                    { 46, false, "", (byte)6, 42, (byte)1, "", "PROFALL", "profall" },
                    { 47, false, "", (byte)4, 42, (byte)1, "", "QNEWPROFLBL", "new-ps-labels" },
                    { 48, false, "", (byte)5, 42, (byte)1, "", "PROFNEW", "profnew" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: 55);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "NAVIGATION",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)")
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");
        }
    }
}
