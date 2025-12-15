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
  YTD_WAGES_EXTRACT_LIVE: "ytd-wages-extract-live",
  DISTRIBUTIONS_BY_AGE: "distributions-by-age",
  CONTRIBUTIONS_BY_AGE: "contributions-by-age",
  FORFEITURES_BY_AGE: "forfeitures-by-age",
  BALANCE_BY_AGE: "balance-by-age",
  BALANCE_BY_YEARS: "balance-by-years",
  VESTED_AMOUNTS_BY_AGE: "vested-amounts-by-age",
  PROF_TERM: "prof-term",
  MILITARY_CONTRIBUTION: "military-contribution",

  DEMOGRAPHIC_BADGES: "demographic-badges-not-in-payprofit",
  DUPLICATE_SSNS: "duplicate-ssns-demographics",
  NEGATIVE_ETVA: "negative-etva-for-ssns-on-payprofit",
  DUPLICATE_NAMES: "duplicate-names-and-birthdays",
  REHIRE_FORFEITURES: "unforfeitures",
  PROFIT_SHARE_REPORT: "profit-share-report",
  PROFIT_SHARE_UPDATE: "profit-share-update",

  FROZEN_SUMMARY: "frozen-summary",
  MASTER_INQUIRY: "master-inquiry",
  ADJUSTMENTS: "adjustments",
  BENEFICIARY_INQUIRY: "beneficiary",
  PAY_BE_NEXT: "adhoc-beneficiaries-report",
  FORFEITURES_ADJUSTMENT: "forfeitures-adjustment",
  PAY_BEN_REPORT: "payben-report",
  DISTRIBUTIONS_INQUIRY: "distributions-inquiry",
  VIEW_DISTRIBUTION: "view-distribution",
  ADD_DISTRIBUTION: "add-distribution",
  EDIT_DISTRIBUTION: "edit-distribution",

  PAY426_SUMMARY: "pay426-9",
  PAY426N_LIVE: "pay426n",
  PAY426N_FROZEN: "pay426n",
  QPAY066_ADHOC: "qpay066-adhoc",
  QPAY066B: "qpay066b",
  QPAY600: "qpay600",
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
  AUDIT_SEARCH: "audit-search",
  ORACLE_HCM_DIAGNOSTICS: "oracle-hcm-diagnostics",
  DEV_DEBUG: "dev-debug",
  DOCUMENTATION: "documentation",
  DIVORCE_REPORT: "divorce-report"
} as const;

export const CAPTIONS = {
  ADD_DISTRIBUTION: "Add Distribution (008-14A)",
  ADD_DISTRIBUTION_SHORT: "Add Distribution",

  ADJUSTMENTS: "Adjustments",
  ADJUSTMENTS_SHORT: "Adjustments",

  AUDIT_SEARCH: "Audit Search",
  AUDIT_SEARCH_SHORT: "Audit Search",

  BALANCE_BY_AGE: "Balance by Age (PROF130B)",
  BALANCE_BY_AGE_SHORT: "Balance by Age",

  BALANCE_BY_YEARS: "Balance by Years (PROF130Y)",
  BALANCE_BY_YEARS_SHORT: "Balance by Years",

  BENEFICIARIES_LIST: "Beneficiaries List",
  BENEIFICAIRES_LIST_SHORT: "Beneficiaries List",

  BENEFICIARY_INQUIRY: "Beneficiary Inquiry",
  BENEFICIARY_INQUIRY_SHORT: "Beneficiary Inquiry",

  BENEFICIARY_OF: "Beneficiary Of",
  BENEFICIARY_OF_SHORT: "Beneficiary Of",

  BENEFICIARY_SEARCH_FILTER: "Beneficiaries",
  BENEFICIARY_SEARCH_FILTER_SHORT: "Beneficiaries",

  BREAKDOWN_REPORT: "Breakdown Report (QPAY066TA)",
  BREAKDOWN_REPORT_SHORT: "Breakdown Report",

  CONTRIBUTIONS_BY_AGE: "Contributions by Age (PROF130)",
  CONTRIBUTIONS_BY_AGE_SHORT: "Contributions by Age",

  DECEMBER: "December",
  DECEMBER_SHORT: "December",

  DECEMBER_ACTIVITIES_SUMMARY: "December Activities Summary",
  DECEMBER_ACTIVITIES_SUMMARY_SHORT: "December Activities Summary",

  DEMOGRAPHIC_BADGES: "Demographic Badges Not In Payprofit",
  DEMOGRAPHIC_BADGES_SHORT: "Demographic Badges",

  DEMOGRAPHIC_FREEZE: "Demographic Freeze",
  DEMOGRAPHIC_FREEZE_SHORT: "Demographic Freeze",

  DEMOGRAPHIC_FREEZE_EDIT: "Edit Demographic Freeze",
  DEMOGRAPHIC_FREEZE_EDIT_SHORT: "Edit Demographic Freeze",

  DEV_DEBUG: "Dev Debug",
  DEV_DEBUG_SHORT: "Dev Debug",

  DISTRIBUTIONS_AND_FORFEITURES: "Distributions and Forfeitures (QPAY129)",
  DISTRIBUTIONS_AND_FORFEITURES_SHORT: "Distributions and Forfeitures",

  DISTRIBUTIONS_BY_AGE: "Distributions by Age (PROF130)",
  DISTRIBUTIONS_BY_AGE_SHORT: "Distributions by Age",

  DISTRIBUTIONS_INQUIRY: "Distribution Inquiry (008-14L)",
  DISTRIBUTIONS_INQUIRY_SHORT: "Distribution Inquiry",

  DIVORCE_REPORT: "Account History Report",
  DIVORCE_REPORT_SHORT: "Account History",

  DOCUMENTATION: "Documentation",
  DOCUMENTATION_SHORT: "Documentation",

  DUPLICATE_NAMES: "Duplicate Names and Birthdays",
  DUPLICATE_NAMES_SHORT: "Duplicate Names and Birthdays",

  DUPLICATE_SSNS: "Duplicate SSNs in Demographics",
  DUPLICATE_SSNS_SHORT: "Duplicate SSNs",

  EDIT_DISTRIBUTION: "Edit Distribution (008-14E)",
  EDIT_DISTRIBUTION_SHORT: "Edit Distribution",

  ELIGIBLE_EMPLOYEES: "Get Eligible Employees",
  ELIGIBLE_EMPLOYEES_SHORT: "Eligible Employees",

  EMPLOYEES_MILITARY: "Employees on Military Leave (QPAY511)",
  EMPLOYEES_MILITARY_SHORT: "Employees on Military Leave",

  FISCAL_CLOSE: MENU_LABELS.FISCAL_CLOSE,
  FISCAL_CLOSE_SHORT: "Fiscal Close",

  FORFEIT: "Forfeit (PAY443)",
  FORFEIT_SHORT: "Forfeit",

  FORFEITURES_ADJUSTMENT: "Forfeitures Adjustment(008-12)",
  FORFEITURES_ADJUSTMENT_SHORT: "Forfeitures Adjustment",

  FORFEITURES_BY_AGE: "Forfeitures by Age (PROF130)",
  FORFEITURES_BY_AGE_SHORT: "Forfeitures by Age",

  FROZEN_SUMMARY: "Frozen Summary",
  FROZEN_SUMMARY_SHORT: "Frozen Summary",

  IT_DEVOPS: "IT DevOps",
  IT_DEVOPS_SHORT: "IT DevOps",

  MANAGE_EXECUTIVE_HOURS: "Manage Executive Hours and Dollars (TPR008-09)",
  MANAGE_EXECUTIVE_HOURS_SHORT: "Manage Executive Hours",

  MASTER_INQUIRY: "Master Inquiry (008-10)",
  MASTER_INQUIRY_SHORT: "Master Inquiry",

  MILITARY_AND_REHIRE: "Military and Rehire",
  MILITARY_AND_REHIRE_SHORT: "Military and Rehire",

  MILITARY_CONTRIBUTIONS: "Military Contributions (TPR008-13)",
  MILITARY_CONTRIBUTIONS_SHORT: "Military Contributions",

  NEGATIVE_ETVA: "Negative ETVA for SSNs on Payprofit",
  NEGATIVE_ETVA_SHORT: "Negative ETVA",

  NEW_PS_LABELS: "New Labels (QNEWPROFLBL)",
  NEW_PS_LABELS_SHORT: "New Labels",

  PAY426_ACTIVE_18_20: "Active/inactive employees age 18 - 20 with 1000 hours or more",
  PAY426_ACTIVE_18_20_SHORT: "Active/inactive 18-20",

  PAY426_ACTIVE_21_PLUS: "Active/inactive employees age 21 & with 1000 hours or more",
  PAY426_ACTIVE_21_PLUS_SHORT: "Active/inactive 21+",

  PAY426_ACTIVE_NO_PRIOR: "Active/inactive employees 18 and older with no prior profit sharing amounts and <1000 hours",
  PAY426_ACTIVE_NO_PRIOR_SHORT: "Active/inactive no prior",

  PAY426_ACTIVE_PRIOR_SHARING:
    "Active/inactive employees 18 and older with prior profit sharing amounts and <1000 hours",
  PAY426_ACTIVE_PRIOR_SHARING_SHORT: "Active/inactive prior sharing",

  PAY426_ACTIVE_UNDER_18: "Active/inactive employees under 18",
  PAY426_ACTIVE_UNDER_18_SHORT: "Active/inactive under 18",

  PAY426_NON_EMPLOYEE: "All non-employee beneficiaries",
  PAY426_NON_EMPLOYEE_SHORT: "Non-employee beneficiaries",

  PAY426_NO_WAGE_EMPLOYEES_POSITIVE_BALANCE: "No Wages Employees Positive Balance",
  PAY426_NO_WAGE_EMPLOYEES_POSITIVE_BALANCE_SHORT: "No Wages Positive Balance",

  PAY426_SUMMARY: "Profit Summary",
  PAY426_SUMMARY_SHORT: "Profit Summary",

  PAY426_TERMINATED_1000_PLUS: "Terminated employees 18 and older with 1000 hours or more",
  PAY426_TERMINATED_1000_PLUS_SHORT: "Terminated 1000+",

  PAY426_TERMINATED_NO_PRIOR: "Terminated employees 18 and older with no prior profit sharing amounts and < 1000 hours",
  PAY426_TERMINATED_NO_PRIOR_SHORT: "Terminated no prior",

  PAY426_TERMINATED_PRIOR: "Terminated employees 18 and older with prior profit sharing amounts and < 1000 hours",
  PAY426_TERMINATED_PRIOR_SHORT: "Terminated prior sharing",

  PAY426_TERMINATED_UNDER_18: "< AGE 18 WAGES > 0 (TERMINATED)",
  PAY426_TERMINATED_UNDER_18_SHORT: "Terminated under 18",

  PAY426N: "Profit Sharing Report (PAY426N)",
  PAY426N_SHORT: "PAY426N",

  PAY450_SUMMARY: "Update Summary (PAY450)",
  PAY450_SUMMARY_SHORT: "Update Summary",

  PAY_BE_NEXT: "Pay Be Next",
  PAY_BE_NEXT_SHORT: "Pay Be Next",

  PAY_SHARE_BY_STORE_REPORTS: "Pay Share By Store Reports",
  PAY_SHARE_BY_STORE_REPORTS_SHORT: "Pay Share By Store",

  PAYBEN_REPORT: "Pay Beneficiary Report",
  PAYBEN_REPORT_SHORT: "Pay Beneficiary Report",

  PAYMASTER_UPDATE: "Profit Master Update (PAY460)",
  PAYMASTER_UPDATE_SHORT: "Profit Master Update",

  POST_FROZEN: "Post Frozen",
  POST_FROZEN_SHORT: "Post Frozen",

  PRINT_PROFIT_CERTS: "Print Profit Certs (PAYCERT)",
  PRINT_PROFIT_CERTS_SHORT: "Print Profit Certs",

  PROF_CTRLSHEET: "Prof-CTRL-SHEET",
  PROF_CTRLSHEET_SHORT: "Prof-CTRL-SHEET",

  PROFALL: "PROFALL Report",
  PROFALL_SHORT: "PROFALL",

  PROFNEW: "PROFNEW",
  PROFNEW_SHORT: "PROFNEW",

  PROFIT_MASTER_UPDATE: "Pay Master Update (PAY460)",
  PROFIT_MASTER_UPDATE_SHORT: "Pay Master Update",

  PROFIT_SHARE_BY_STORE: "Profit Share by Store (QPAY066TA)",
  PROFIT_SHARE_BY_STORE_SHORT: "Profit Share by Store",

  PROFIT_SHARE_GROSS_REPORT: "Profit Share Gross Report (QPAY501)",
  PROFIT_SHARE_GROSS_REPORT_SHORT: "Profit Share Gross Report",

  PROFIT_SHARE_REPORT: "Profit Share Report (PAY426)",
  PROFIT_SHARE_REPORT_SHORT: "Profit Share Report",

  PROFIT_SHARE_REPORT_EDIT_RUN: "Profit Share Report Edit Run",
  PROFIT_SHARE_REPORT_EDIT_RUN_SHORT: "Profit Share Report Edit Run",

  PROFIT_SHARE_REPORT_FINAL_RUN: "Profit Share Report Final Run (PAY426N)",
  PROFIT_SHARE_REPORT_FINAL_RUN_SHORT: "Profit Share Report Final Run",

  PROFIT_SHARE_TOTALS: "Profit Share Totals Report (PAY426)",
  PROFIT_SHARE_TOTALS_SHORT: "Profit Share Totals",

  PROFIT_SHARE_UPDATE: "Master Update (PAY444|PAY447)",
  PROFIT_SHARE_UPDATE_SHORT: "Master Update",

  PROFIT_SHARING_CONTROL_SHEET: "Profit Sharing Control Sheet",
  PROFIT_SHARING_CONTROL_SHEET_SHORT: "Control Sheet",

  QPAY066_ADHOC: "QPAY066* Ad Hoc Reports",
  QPAY066_ADHOC_SHORT: "QPAY066* Reports",

  QPAY066_UNDER21: "QPAY066-UNDR21",
  QPAY066_UNDER21_SHORT: "QPAY066 Under 21",

  QPAY066B: "QPAY066B",
  QPAY066B_SHORT: "QPAY066B",

  QPAY066TA: "QPAY066TA",
  QPAY066TA_SHORT: "QPAY066TA",

  QPAY066TA_UNDER21: "QPAY066TA-UNDR21",
  QPAY066TA_UNDER21_SHORT: "QPAY066TA Under 21",

  QPAY600: "QPAY600",
  QPAY600_SHORT: "QPAY600",

  RECENTLY_TERMINATED: "Recently Terminated (PROF-VESTED|PAY508)",
  RECENTLY_TERMINATED_SHORT: "Recently Terminated",

  REHIRE_FORFEITURES: "UnForfeit (QPREV-PROF)",
  REHIRE_FORFEITURES_SHORT: "UnForfeit",

  REPRINT_CERTIFICATES: "Reprint Certificates",
  REPRINT_CERTIFICATES_SHORT: "Reprint Certificates",

  TERMINATED_LETTERS: "Terminated Letters (QPROF003-1)",
  TERMINATED_LETTERS_SHORT: "Terminated Letters",

  TERMINATIONS: "Terminations (QPAY066)",
  TERMINATIONS_SHORT: "Terminations",

  VESTED_AMOUNTS_BY_AGE: "Vested Amounts by Age (PROF130V)",
  VESTED_AMOUNTS_BY_AGE_SHORT: "Vested Amounts by Age",

  VIEW_DISTRIBUTION: "View Distribution (008-14V)",
  VIEW_DISTRIBUTION_SHORT: "View Distribution",

  YTD_WAGES_EXTRACT: "YTD Wages Extract (PROF-DOLLAR-EXTRACT)",
  YTD_WAGES_EXTRACT_SHORT: "YTD Wages Extract",

  YTD_WAGES_EXTRACT_LIVE: "YTD Wages Extract Live (PROF-DOLLAR-EXTRACT)",
  YTD_WAGES_EXTRACT_LIVE_SHORT: "YTD Wages Extract Live"
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
