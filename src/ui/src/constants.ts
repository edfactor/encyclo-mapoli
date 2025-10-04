export const MAX_EMPLOYEE_BADGE_LENGTH: number = 7;

export const drawerOpenWidth: number = 368;
export const drawerClosedWidth: number = 64;

export const MENU_LABELS = {
  FISCAL_CLOSE: "Fiscal Close",
  DECEMBER_ACTIVITIES: "December Activities",
  BENEFICIARIES: "Beneficiaries",
  DISTRIBUTIONS: "Distributions",
  RECONCILIATION: "Reconciliation",
  INQUIRIES: "Inquiries",
  IT_DEVOPS: "IT DEVOPS",
  IT_OPERATIONS: "IT OPERATIONS",
  GO_TO_PROFIT_SHARE_REPORT: "View Profit Share Report (PAY426) Details",
  YEAR_END: "Year End"
} as const;

export const ROUTES = {
  FISCAL_CLOSE: "fiscal-close",
  DISTRIBUTIONS_AND_FORFEITURES: "distributions-and-forfeitures",
  MANAGE_EXECUTIVE_HOURS: "manage-executive-hours-and-dollars",
  ELIGIBLE_EMPLOYEES: "eligible-employees",
  YTD_WAGES_EXTRACT: "ytd-wages-extract",
  DISTRIBUTIONS_BY_AGE: "distributions-by-age",
  CONTRIBUTIONS_BY_AGE: "contributions-by-age",
  FORFEITURES_BY_AGE: "forfeitures-by-age",
  BALANCE_BY_AGE: "balance-by-age",
  BALANCE_BY_YEARS: "balance-by-years",
  VESTED_AMOUNTS_BY_AGE: "vested-amounts-by-age",
  PROF_TERM: "prof-term",
  MILITARY_CONTRIBUTION: "military-contribution",

  DECEMBER_PROCESS_ACCORDION: "december-process-accordion",
  DEMOGRAPHIC_BADGES: "demographic-badges-not-in-payprofit",
  DUPLICATE_SSNS: "duplicate-ssns-demographics",
  NEGATIVE_ETVA: "negative-etva-for-ssns-on-payprofit",
  DUPLICATE_NAMES: "duplicate-names-and-birthdays",
  MISSING_COMMA: "missing-comma-in-py-name",
  REHIRE_FORFEITURES: "unforfeitures",
  PROFIT_SHARE_REPORT: "profit-share-report",
  PROFIT_SHARE_TOTALS: "profit-share-totals",
  PROFIT_SHARE_UPDATE: "profit-share-update",
  PROFNEW: "/",

  FROZEN_SUMMARY: "frozen-summary",
  MASTER_INQUIRY: "master-inquiry",
  BENEFICIARY_INQUIRY: "beneficiary",
  PAY_BE_NEXT: "adhoc-beneficiaries-report",
  FORFEITURES_ADJUSTMENT: "forfeitures-adjustment",
  PAY_BEN_REPORT: "payben-report",
  DISTRIBUTIONS_INQUIRY: "distributions-inquiry",

  PAY426_ACTIVE_18_20: "pay426-1",
  PAY426_ACTIVE_21_PLUS: "pay426-2",
  PAY426_ACTIVE_UNDER_18: "pay426-3",
  PAY426_ACTIVE_PRIOR_SHARING: "pay426-4",
  PAY426_ACTIVE_NO_PRIOR: "pay426-5",
  PAY426_TERMINATED_1000_PLUS: "pay426-6",
  PAY426_TERMINATED_NO_PRIOR: "pay426-7",
  PAY426_TERMINATED_PRIOR: "pay426-8",
  PAY426_SUMMARY: "pay426-9",
  PAY426_NON_EMPLOYEE: "pay426-10",
  PAY426N: "pay426n",
  QPAY066_ADHOC: "qpay066-adhoc",
  QPAY066B: "qpay066b",
  QPAY600: "qpay600",
  REPRINT_CERTIFICATES: "reprint-certificates",
  FORFEIT: "forfeit",
  PROFIT_MASTER_UPDATE: "profit-master-update",
  PROFIT_SHARE_BY_STORE: "profit-share-by-store",
  PROFIT_SHARE_GROSS_REPORT: "profit-share-gross-report",
  RECENTLY_TERMINATED: "recently-terminated",
  TERMINATED_LETTERS: "terminated-letters",
  PAY450_SUMMARY: "pay450-summary",
  PROF_CTRLSHEET: "prof-control-sheet",
  PRINT_PROFIT_CERTS: "print-profit-certs",
  UNDER_21_REPORT: "under-21-report",
  QPAY066_UNDER21: "qpay066-under21",
  QPAY066TA_UNDER21: "qpay066ta-under21",
  QPAY066TA: "qpay066ta",
  NEW_PS_LABELS: "new-ps-labels",
  PROFALL: "profall",
  DEMO_FREEZE: "demographic-freeze",
  DEV_DEBUG: "dev-debug",
  DOCUMENTATION: "documentation",
  PROFIT_SHARE_EDIT: "profit-share-edit",
  SAVE_PROF_PAYMSTR: "save-prof-paymstr"
} as const;

export const CAPTIONS = {
  PAY_BE_NEXT: "Pay Be Next",
  BENEFICIARY_INQUIRY: "Beneficiary Inquiry",
  BENEFICIARY_OF: "Beneficiary Of",
  BENEFICIARY_SEARCH_FILTER: "Beneficiaries",
  PAYBEN_REPORT: "Pay Beneficiary Report",
  BALANCE_BY_AGE: "Get Balance by Age (PROF130B)",
  BALANCE_BY_YEARS: "Get Balance by Years (PROF130Y)",
  CONTRIBUTIONS_BY_AGE: "Get Contributions by Age (PROF130)",
  CLEAN_UP_REPORTS: "Clean Up Reports",
  DEMOGRAPHIC_BADGES: "Demographic Badges Not In Payprofit",
  DECEMBER: "December",
  FISCAL_CLOSE: MENU_LABELS.FISCAL_CLOSE,
  POST_FROZEN: "Post Frozen",
  DISTRIBUTIONS_AND_FORFEITURES: "Distributions and Forfeitures (QPAY129)",
  DISTRIBUTIONS_BY_AGE: "Get Distributions by Age (PROF130)",
  DISTRIBUTIONS_INQUIRY: "Distribution Inquiry (008-14l)",
  DUPLICATE_NAMES: "Duplicate Names and Birthdays",
  DUPLICATE_SSNS: "Duplicate SSNs in Demographics",
  ELIGIBLE_EMPLOYEES: "Get Eligible Employees (GET-ELIGIBLE-EMPS)",
  EMPLOYEES_MILITARY: "Employees on Military Leave (QPAY511)",
  FORFEIT: "Forfeit (PAY443)",
  FORFEITURES_BY_AGE: "Get Forfeitures by Age (PROF130)",
  FORFEITURES_ADJUSTMENT: "Forfeitures Adjustment(008-12)",
  MANAGE_EXECUTIVE_HOURS: "Manage Executive Hours and Dollars (TPR008-09)",
  MASTER_INQUIRY: "Master Inquiry",
  MILITARY_ENTRY_AND_MODIFICATION: "Military Entry and Modification",
  MILITARY_CONTRIBUTIONS: "Military Contributions (TPR008-13)",
  REHIRE_FORFEITURES: "UnForfeit (QPREV-PROF)",
  MISSING_COMMA: "Missing Comma in Full Name",
  NEGATIVE_ETVA: "Negative ETVA",
  NEW_PS_LABELS: "New Labels (QNEWPROFLBL)",
  PAY_SHARE_BY_STORE_REPORTS: "Pay Share By Store Reports",
  PAY426_ACTIVE_18_20: "Active/inactive employees age 18 - 20 with 1000 hours or more",
  PAY426_ACTIVE_21_PLUS: "Active/inactive employees age 21 & with 1000 hours or more",
  PAY426_ACTIVE_NO_PRIOR: "Active/inactive employees 18 and older with no prior profit sharing amounts and <1000 hours",
  PAY426_ACTIVE_PRIOR_SHARING:
    "Active/inactive employees 18 and older with prior profit sharing amounts and <1000 hours",
  PAY426_ACTIVE_UNDER_18: "Active/inactive employees under 18",
  PAY426_TERMINATED_UNDER_18: "< AGE 18 NO WAGES",
  PAY426_NON_EMPLOYEE: "All non-employee beneficiaries",
  PAY426_SUMMARY: "Profit sharing summary page",
  PAY426_TERMINATED_1000_PLUS: "Terminated employees 18 and older with 1000 hours or more",
  PAY426_TERMINATED_NO_PRIOR: "Terminated employees 18 and older with no prior profit sharing amounts and < 1000 hours",
  PAY426_TERMINATED_PRIOR: "Terminated employees 18 and older with prior profit sharing amounts and < 1000 hours",
  PAY426N: "Profit Sharing Report (PAY426N)",
  QPAY066_ADHOC: "QPAY066* Ad Hoc Reports",
  QPAY066B: "QPAY066B",
  QPAY600: "QPAY600",
  REPRINT_CERTIFICATES: "Reprint Certificates",
  PAY450_SUMMARY: "Update Summary (PAY450)",
  PAY450: "PAY450",
  PAYMASTER_UPDATE: "Profit Master Update (PAY460)",
  PROFIT_MASTER_UPDATE: "Pay Master Update (PAY460)",
  PROF_CTRLSHEET: "Prof-CTRL-SHEET",
  PROFALL: "PROFALL Report",
  PROFNEW: "PROFNEW",
  PROFIT_SHARE_BY_STORE: "Profit Share by Store (QPAY066TA)",
  PROFIT_SHARE_GROSS_REPORT: "Profit Share Gross Report (QPAY501)",
  PROFIT_SHARE_REPORT_FINAL_RUN: "Profit Share Report Final Run (PAY426N)",
  PROFIT_SHARE_REPORT: "Profit Share Report (PAY426)",
  PROFIT_SHARE_TOTALS: "Profit Share Totals Report (PAY426)",
  PROFIT_SHARE_UPDATE: "Profit Share Updates (PAY444|PAY447)",
  PROFIT_SHARING_CONTROL_SHEET: "Profit Sharing Control Sheet",
  PRINT_PROFIT_CERTS: "Print Profit Certs (PAYCERT)",
  QPAY066_UNDER21: "QPAY066-UNDR21",
  QPAY066TA_UNDER21: "QPAY066TA-UNDR21",
  QPAY066TA: "QPAY066TA",
  RECENTLY_TERMINATED: "Recently Terminated (PROF-VESTED|PAY508)",
  TERMINATED_LETTERS: "Terminated Letters (QPROF003-1)",
  REPORTS_BY_AGE: "Reports by Age",
  SUMMARY: "Summary",
  TERMINATIONS: "Terminations (QPAY066)",
  VESTED_AMOUNTS_BY_AGE: "Get Vested Amounts by Age (PROF130V)",
  YTD_WAGES_EXTRACT: "YTD Wages Extract (PROF-DOLLAR-EXTRACT)",
  BREAKDOWN_REPORT: "Breakdown Report (QPAY066TA)",
  DEMOGRAPHIC_FREEZE: "Demographic Freeze",
  DEV_DEBUG: "Dev Debug"
} as const;

export const VIEW = "View";

export const SMART_PS_QA_IMPERSONATION = "SMART-PS-QA-Impersonation";
export const SMART_ROLE_PREFIX = "SMART";
export const APP_NAME = "PS";
export const IMPERSONATION_ROLE_SUFFIX = "Impersonation";

export const GRID_COLUMN_WIDTHS = {
  BADGE_NUMBER: 100,
  SSN: 125,
  FULL_NAME: 150
} as const;

/* 
Report ids are:
1: AGE 18-20 WITH >= 1000 PS HOURS 
2: >= AGE 21 WITH >= 1000 PS HOURS 
3: < AGE 18 
4: >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT 
5: >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT 
6: >= AGE 18 WITH >= 1000 PS HOURS (TERMINATED) 
7: >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT (TERMINATED) 
8: >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT (TERMINATED) 
10: < AGE 18 NO WAGES : 0 (TERMINATED)
*/
export const PAY426_REPORT_IDS = {
  EIGHTEEN_TO_TWENTY: 1,
  TWENTY_ONE_PLUS: 2,
  UNDER_EIGHTEEN: 3,
  FEWER_THAN_1000_PRIOR_PS: 4,
  FEWER_THAN_1000_NO_PRIOR_PS: 5,
  TERMINATED_1000_PLUS: 6,
  TERMINATED_NO_PRIOR_PS: 7,
  TERMINATED_WITH_PRIOR_PS: 8,
  TERMINATED_UNDER_EIGHTEEN_NO_WAGES: 10
} as const;

// Mirror of server-side NavigationStatus.Constants for client logic
export const NAVIGATION_STATUS = {
  NOT_STARTED: 1,
  IN_PROGRESS: 2,
  ON_HOLD: 3,
  COMPLETE: 4
} as const;
