using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class shrinkNavigationPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<short>(
                name: "NAVIGATION_ID",
                table: "NAVIGATION_TRACKING",
                type: "NUMBER(5)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<short>(
                name: "NAVIGATIONID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                type: "NUMBER(5)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<short>(
                name: "PARENT_ID",
                table: "NAVIGATION",
                type: "NUMBER(5)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "ID",
                table: "NAVIGATION",
                type: "NUMBER(5)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[,]
                {
                    { (short)50, false, "", (byte)1, null, (byte)1, "", "INQUIRIES", "" },
                    { (short)52, true, "", (byte)2, null, (byte)1, "", "BENEFICIARIES", "" },
                    { (short)53, true, "", (byte)3, null, (byte)1, "", "DISTRIBUTIONS", "" },
                    { (short)54, true, "", (byte)4, null, (byte)1, "", "RECONCILIATION", "" },
                    { (short)55, false, "", (byte)5, null, (byte)1, "", "YEAR END", "" },
                    { (short)56, false, "", (byte)6, null, (byte)1, "", "IT OPERATIONS", "" },
                    { (short)32767, false, "", (byte)1, null, (byte)1, "", "Unknown", "" }
                });

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "NAME",
                value: "System-Administrator");

            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[,]
                {
                    { (short)1, false, "", (byte)1, (short)55, (byte)1, "", "December Activities", "december-process-accordion" },
                    { (short)14, false, "", (byte)2, (short)55, (byte)1, "", "Fiscal Close", "fiscal-close" },
                    { (short)51, false, "", (byte)1, (short)50, (byte)1, "", "MASTER INQUIRY", "master-inquiry" },
                    { (short)57, false, "", (byte)1, (short)56, (byte)1, "", "Demographic Freeze", "demographic-freeze" },
                    { (short)2, false, "", (byte)1, (short)1, (byte)1, "", "Clean up Reports", "" },
                    { (short)7, false, "", (byte)3, (short)1, (byte)1, "008-13", "Military Contributions", "military-entry-and-modification" },
                    { (short)8, false, "", (byte)2, (short)1, (byte)1, "QPREV-PROF", "Unforfeit", "unforfeitures" },
                    { (short)9, false, "", (byte)4, (short)1, (byte)1, "QPAY066", "Terminations", "prof-term" },
                    { (short)10, false, "", (byte)5, (short)1, (byte)1, "008-12", "Forfeitures", "forfeitures-adjustment" },
                    { (short)11, false, "", (byte)6, (short)1, (byte)1, "QPAY129", "Distributions and Forfeitures", "distributions-and-forfeitures" },
                    { (short)13, false, "", (byte)9, (short)1, (byte)1, "PAY426", "Profit Share Report", "profit-share-report" },
                    { (short)15, false, "", (byte)1, (short)14, (byte)1, "PROF-DOLLAR-EXEC-EXTRACT, TPR008-09", "Manage Executive Hours", "manage-executive-hours-and-dollars" },
                    { (short)16, false, "", (byte)2, (short)14, (byte)1, "PROF-DOLLAR-EXTRACT", "YTD Wages Extract", "ytd-wages-extract" },
                    { (short)17, false, "", (byte)4, (short)14, (byte)1, "PAY426", "Profit Share Report (Final Run)", "profit-share-report" },
                    { (short)18, false, "", (byte)3, (short)14, (byte)1, "PAY426", "Profit Share Report (Edit Run)", "pay426n" },
                    { (short)30, false, "", (byte)5, (short)14, (byte)1, "GET-ELIGIBLE-EMPS", "Get Eligible Employees", "eligible-employees" },
                    { (short)31, false, "", (byte)6, (short)14, (byte)1, "PAY443", "Profit Share Forfeit", "forfeit" },
                    { (short)33, false, "", (byte)10, (short)14, (byte)1, "PAY450", "Prof PayMaster Update", "pay450-summary" },
                    { (short)34, false, "", (byte)12, (short)14, (byte)1, "Prof130", "Prof Share Report By Age", "" },
                    { (short)41, false, "", (byte)13, (short)14, (byte)1, "QPAY501", "Prof Share Gross Rpt", "profit-share-gross-report" },
                    { (short)42, false, "", (byte)14, (short)14, (byte)1, "QPAY066TA", "Prof Share by Store", "" },
                    { (short)49, false, "", (byte)15, (short)14, (byte)1, "PAYCERT", "Print Profit Certs", "print-profit-certs" },
                    { (short)60, false, "", (byte)7, (short)14, (byte)1, "PAY444|PAY447", "Master Update", "profit-share-update" },
                    { (short)62, true, "", (byte)8, (short)14, (byte)1, "PAY460, PROFTLD", "Profit Master Update", "profit-master-update" },
                    { (short)63, false, "", (byte)16, (short)14, (byte)1, "", "Save Prof Paymstr", "save-prof-paymstr" },
                    { (short)64, false, "", (byte)11, (short)14, (byte)1, "PROF-CNTRL-SHEET", "Prof Control Sheet", "prof-control-sheet" },
                    { (short)65, false, "", (byte)17, (short)14, (byte)1, "QPAY066*", "QPAY066* Ad Hoc Reports", "qpay066-adhoc" },
                    { (short)3, false, "", (byte)1, (short)2, (byte)1, "", "Demographic Badges Not In PayProfit", "demographic-badges-not-in-payprofit" },
                    { (short)4, false, "", (byte)2, (short)2, (byte)1, "", "Duplicate SSNs in Demographics", "duplicate-ssns-demographics" },
                    { (short)5, false, "", (byte)3, (short)2, (byte)1, "", "Negative ETVA", "negative-etva-for-ssns-on-payprofit" },
                    { (short)6, false, "", (byte)4, (short)2, (byte)1, "", "Duplicate Names and Birthdays", "duplicate-names-and-birthdays" },
                    { (short)35, false, "", (byte)2, (short)34, (byte)1, "PROF130", "CONTRIBUTIONS BY AGE", "contributions-by-age" },
                    { (short)36, false, "", (byte)1, (short)34, (byte)1, "PROF130", "DISTRIBUTIONS BY AGE", "distributions-by-age" },
                    { (short)37, false, "", (byte)3, (short)34, (byte)1, "PROF130", "FORFEITURES BY AGE", "forfeitures-by-age" },
                    { (short)38, false, "", (byte)4, (short)34, (byte)1, "PROF130B", "BALANCE BY AGE", "balance-by-age" },
                    { (short)39, false, "", (byte)5, (short)34, (byte)1, "PROF130V", "VESTED AMOUNTS BY AGE", "vested-amounts-by-age" },
                    { (short)40, false, "", (byte)6, (short)34, (byte)1, "PROF130Y", "BALANCE BY YEARS", "balance-by-years" },
                    { (short)43, false, "", (byte)1, (short)42, (byte)1, "", "QPAY066-UNDR21", "qpay066-under21" },
                    { (short)44, false, "", (byte)2, (short)42, (byte)1, "", "QPAY066TA-UNDR21", "qpay066ta-under21" },
                    { (short)45, false, "", (byte)3, (short)42, (byte)1, "", "QPAY066TA", "qpay066ta" },
                    { (short)46, false, "", (byte)6, (short)42, (byte)1, "", "PROFALL", "profall" },
                    { (short)47, false, "", (byte)4, (short)42, (byte)1, "", "QNEWPROFLBL", "new-ps-labels" },
                    { (short)48, false, "", (byte)5, (short)42, (byte)1, "", "PROFNEW", "profnew" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "NAVIGATION_ID",
                table: "NAVIGATION_TRACKING",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "NUMBER(5)");

            migrationBuilder.AlterColumn<int>(
                name: "NAVIGATIONID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "NUMBER(5)");

            migrationBuilder.AlterColumn<int>(
                name: "PARENT_ID",
                table: "NAVIGATION",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "NUMBER(5)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "NAVIGATION",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "NUMBER(5)");

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
                    { 56, false, "", (byte)6, null, (byte)1, "", "IT OPERATIONS", "" }
                });

            migrationBuilder.UpdateData(
                table: "NAVIGATION_ROLE",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "NAME",
                value: "Profit-Sharing-Administrator");

            migrationBuilder.InsertData(
                table: "NAVIGATION",
                columns: new[] { "ID", "DISABLED", "ICON", "ORDER_NUMBER", "PARENT_ID", "STATUS_ID", "SUB_TITLE", "TITLE", "URL" },
                values: new object[,]
                {
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
    }
}
