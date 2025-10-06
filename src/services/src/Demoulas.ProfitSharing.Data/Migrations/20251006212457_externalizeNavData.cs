using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class externalizeNavData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)4);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)5);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)6);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)7);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)8);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)9);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)10);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)11);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)13);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)15);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)16);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)17);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)18);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)30);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)31);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)33);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)35);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)36);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)37);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)38);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)39);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)40);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)41);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)43);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)44);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)45);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)46);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)47);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)48);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)49);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)51);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)52);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)53);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)54);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)57);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)60);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)62);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)63);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)64);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)65);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)66);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)67);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)68);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)69);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)32767);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)34);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)42);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)50);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)56);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)14);

            migrationBuilder.DeleteData(
                table: "NAVIGATION",
                keyColumn: "ID",
                keyValue: (short)55);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "IS_NAVIGABLE", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[,]
                {
                    { (short)50, false, "", null, (byte)1, null, (byte)1, "", "INQUIRIES", "" },
                    { (short)52, true, "", null, (byte)2, null, (byte)1, "", "BENEFICIARIES", "" },
                    { (short)53, true, "", null, (byte)3, null, (byte)1, "", "DISTRIBUTIONS", "" },
                    { (short)54, true, "", null, (byte)4, null, (byte)1, "", "RECONCILIATION", "" },
                    { (short)55, false, "", null, (byte)5, null, (byte)1, "", "YEAR END", "" },
                    { (short)56, false, "", null, (byte)6, null, (byte)1, "", "IT DEVOPS", "" },
                    { (short)32767, false, "", null, (byte)1, null, (byte)1, "", "Unknown", "" },
                    { (short)1, false, "", null, (byte)1, (short)55, (byte)1, "", "December Activities", "december-process-accordion" },
                    { (short)14, false, "", null, (byte)2, (short)55, (byte)1, "", "Fiscal Close", "fiscal-close" },
                    { (short)51, false, "", null, (byte)1, (short)50, (byte)1, "", "MASTER INQUIRY", "master-inquiry" },
                    { (short)57, false, "", null, (byte)1, (short)56, (byte)1, "", "Demographic Freeze", "demographic-freeze" },
                    { (short)2, false, "", null, (byte)1, (short)1, (byte)1, "", "Clean up Reports", "" },
                    { (short)7, false, "", null, (byte)3, (short)1, (byte)1, "008-13", "Military Contributions", "military-entry-and-modification" },
                    { (short)8, false, "", null, (byte)2, (short)1, (byte)1, "QPREV-PROF", "Unforfeit", "unforfeitures" },
                    { (short)9, false, "", null, (byte)4, (short)1, (byte)1, "QPAY066", "Terminations", "prof-term" },
                    { (short)10, false, "", null, (byte)5, (short)1, (byte)1, "008-12", "Forfeitures", "forfeitures-adjustment" },
                    { (short)11, false, "", null, (byte)6, (short)1, (byte)1, "QPAY129", "Distributions and Forfeitures", "distributions-and-forfeitures" },
                    { (short)13, false, "", null, (byte)9, (short)1, (byte)1, "PAY426", "Profit Share Report", "profit-share-report" },
                    { (short)15, false, "", null, (byte)1, (short)14, (byte)1, "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09", "Manage Executive Hours", "manage-executive-hours-and-dollars" },
                    { (short)16, false, "", null, (byte)2, (short)14, (byte)1, "PROF-DOLLAR-EXTRACT", "YTD Wages Extract", "ytd-wages-extract" },
                    { (short)17, false, "", null, (byte)4, (short)14, (byte)1, "PAY426", "Profit Share Report (Final Run)", "profit-share-report" },
                    { (short)18, false, "", null, (byte)3, (short)14, (byte)1, "PAY426", "Profit Share Report (Edit Run)", "pay426n" },
                    { (short)30, false, "", null, (byte)5, (short)14, (byte)1, "GET-ELIGIBLE-EMPS", "Get Eligible Employees", "eligible-employees" },
                    { (short)31, false, "", null, (byte)6, (short)14, (byte)1, "PAY443", "Profit Share Forfeit", "forfeit" },
                    { (short)33, false, "", null, (byte)10, (short)14, (byte)1, "PAY450", "Prof PayMaster Update", "pay450-summary" },
                    { (short)34, false, "", null, (byte)12, (short)14, (byte)1, "Prof130", "Prof Share Report By Age", "" },
                    { (short)41, false, "", null, (byte)13, (short)14, (byte)1, "QPAY501", "Prof Share Gross Rpt", "profit-share-gross-report" },
                    { (short)42, false, "", null, (byte)14, (short)14, (byte)1, "QPAY066TA", "Prof Share by Store", "" },
                    { (short)49, false, "", null, (byte)15, (short)14, (byte)1, "PAYCERT", "Print Profit Certs", "print-profit-certs" },
                    { (short)60, false, "", null, (byte)7, (short)14, (byte)1, "PAY444|PAY447", "Master Update", "profit-share-update" },
                    { (short)62, true, "", null, (byte)8, (short)14, (byte)1, "PAY460, PROFTLD", "Profit Master Update", "profit-master-update" },
                    { (short)63, false, "", null, (byte)16, (short)14, (byte)1, "", "Save Prof Paymstr", "save-prof-paymstr" },
                    { (short)64, false, "", null, (byte)11, (short)14, (byte)1, "PROF-CNTRL-SHEET", "Prof Control Sheet", "prof-control-sheet" },
                    { (short)65, false, "", null, (byte)17, (short)14, (byte)1, "QPAY066*", "QPAY066* Ad Hoc Reports", "qpay066-adhoc" },
                    { (short)66, false, "", null, (byte)18, (short)14, (byte)1, "PROF-VESTED|PAY508", "Recently Terminated", "recently-terminated" },
                    { (short)67, false, "", null, (byte)10, (short)1, (byte)1, "", "Pay Beneficiary Report", "payben-report" },
                    { (short)68, false, "", null, (byte)18, (short)14, (byte)1, "", "Adhoc Beneficiaries Report", "adhoc-beneficiaries-report" },
                    { (short)69, false, "", null, (byte)19, (short)14, (byte)1, "QPROF003-1", "Terminated Letters", "terminated-letters" },
                    { (short)3, false, "", null, (byte)1, (short)2, (byte)1, "", "Demographic Badges Not In PayProfit", "demographic-badges-not-in-payprofit" },
                    { (short)4, false, "", null, (byte)2, (short)2, (byte)1, "", "Duplicate SSNs in Demographics", "duplicate-ssns-demographics" },
                    { (short)5, false, "", null, (byte)3, (short)2, (byte)1, "", "Negative ETVA", "negative-etva-for-ssns-on-payprofit" },
                    { (short)6, false, "", null, (byte)4, (short)2, (byte)1, "", "Duplicate Names and Birthdays", "duplicate-names-and-birthdays" },
                    { (short)35, false, "", null, (byte)2, (short)34, (byte)1, "PROF130", "CONTRIBUTIONS BY AGE", "contributions-by-age" },
                    { (short)36, false, "", null, (byte)1, (short)34, (byte)1, "PROF130", "DISTRIBUTIONS BY AGE", "distributions-by-age" },
                    { (short)37, false, "", null, (byte)3, (short)34, (byte)1, "PROF130", "FORFEITURES BY AGE", "forfeitures-by-age" },
                    { (short)38, false, "", null, (byte)4, (short)34, (byte)1, "PROF130B", "BALANCE BY AGE", "balance-by-age" },
                    { (short)39, false, "", null, (byte)5, (short)34, (byte)1, "PROF130V", "VESTED AMOUNTS BY AGE", "vested-amounts-by-age" },
                    { (short)40, false, "", null, (byte)6, (short)34, (byte)1, "PROF130Y", "BALANCE BY YEARS", "balance-by-years" },
                    { (short)43, false, "", null, (byte)1, (short)42, (byte)1, "", "QPAY066-UNDR21", "qpay066-under21" },
                    { (short)44, false, "", null, (byte)2, (short)42, (byte)1, "", "QPAY066TA-UNDR21", "qpay066ta-under21" },
                    { (short)45, false, "", null, (byte)3, (short)42, (byte)1, "", "QPAY066TA", "qpay066ta" },
                    { (short)46, false, "", null, (byte)6, (short)42, (byte)1, "", "PROFALL", "profall" },
                    { (short)47, false, "", null, (byte)4, (short)42, (byte)1, "", "QNEWPROFLBL", "new-ps-labels" },
                    { (short)48, false, "", null, (byte)5, (short)42, (byte)1, "", "PROFNEW", "profnew" }
                });
        }
    }
}
