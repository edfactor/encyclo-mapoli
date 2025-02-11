export const HISTORY_KEY = 'navigation_history' as const;

export const MENU_LABELS = {
  FISCAL_CLOSE: "Fiscal Close",
  DECEMBER_FLOW: "December Flow",
  BENEFICIARIES: "Beneficiaries",
  DISTRIBUTIONS: "Distributions",
  RECONCILIATION: "Reconciliation",
  INQUIRIES: "Inquiries",
} as const;

export const ROUTES = {
  FISCAL_CLOSE: "fiscal-close",
  DISTRIBUTIONS_AND_FORFEITURES: "distributions-and-forfeitures",
  MANAGE_EXECUTIVE_HOURS: "manage-executive-hours-and-dollars",
  ELIGIBLE_EMPLOYEES: "eligible-employees",
  DISTRIBUTIONS_BY_AGE: "distributions-by-age",
  CONTRIBUTIONS_BY_AGE: "contributions-by-age",
  FORFEITURES_BY_AGE: "forfeitures-by-age",
  BALANCE_BY_AGE: "balance-by-age",
  BALANCE_BY_YEARS: "balance-by-years",
  VESTED_AMOUNTS_BY_AGE: "vested-amounts-by-age",
  PROF_TERM: "prof-term",
  MILITARY_AND_REHIRE_ENTRY: "military-and-rehire-entry",

  DECEMBER_PROCESS_ACCORDION: "december-process-accordion",
  DEMOGRAPHIC_BADGES: "demographic-badges-not-in-payprofit",
  DUPLICATE_SSNS: "duplicate-ssns-demographics",
  NEGATIVE_ETVA: "negative-etva-for-ssns-on-payprofit",
  DUPLICATE_NAMES: "duplicate-names-and-birthdays",
  MISSING_COMMA: "missing-comma-in-py-name",
  EMPLOYEES_MILITARY: "employees-on-military-leave",
  MILITARY_FORFEITURES: "military-and-rehire-forfeitures",
  MILITARY_PROFIT_SUMMARY: "military-and-rehire-profit-summary",
  PROFIT_SHARE_REPORT: "profit-share-report",
  PROFIT_SHARE_REPORT_EDIT_RUN: "profit-share-report-edit-run",
  PROFIT_SHARE_UPDATE: "profit-share-update",

  FROZEN_SUMMARY: "frozen-summary",
  MASTER_INQUIRY: "master-inquiry",

  PAY426_ACTIVE_18_20: "pay426-1",
  PAY426_ACTIVE_21_PLUS: "pay426-2",
  PAY426_ACTIVE_UNDER_18: "pay426-3",
  PAY426_ACTIVE_PRIOR_SHARING: "pay426-4",
  PAY426_ACTIVE_NO_PRIOR: "pay426-5",
  PAY426_TERMINATED_1000_PLUS: "pay426-6",
  PAY426_TERMINATED_NO_PRIOR: "pay426-7",
  PAY426_TERMINATED_PRIOR: "pay426-8",
  PAY426_SUMMARY: "pay426-9",
  PAY426_NON_EMPLOYEE: "pay426-10"
} as const;

export const CAPTIONS = {
  DISTRIBUTIONS_AND_FORFEITURES: "Distributions and Forfeitures",
  MANAGE_EXECUTIVE_HOURS: "Manage Executive Hours and Dollars",
  ELIGIBLE_EMPLOYEES: "Get Eligible Employees",
  DISTRIBUTIONS_BY_AGE: "Get Distributions by Age (PROF 130)",
  CONTRIBUTIONS_BY_AGE: "Get Contributions by Age (PROF 130)",
  FORFEITURES_BY_AGE: "Get Forfeitures by Age (PROF 130)",
  BALANCE_BY_AGE: "Get Balance by Age (PROF 130B)",
  BALANCE_BY_YEARS: "Get Balance by Years (PROF 130Y)",
  VESTED_AMOUNTS_BY_AGE: "Get Vested Amounts by Age (PROF 130V)",
  TERMINATIONS: "Terminations (QPAY066)",
  MILITARY_AND_REHIRE_ENTRY: "Military and Rehire Entry",
  SUMMARY: "Summary",
  DEMOGRAPHIC_BADGES: "Demographic Badges Not In Payprofit",
  DUPLICATE_SSNS: "Duplicate SSNs in Demographics",
  NEGATIVE_ETVA: "Negative ETVA for SSNs on Payprofit",
  DUPLICATE_NAMES: "Duplicate Names and Birthdays",
  MISSING_COMMA: "Missing Comma in Full Name",
  EMPLOYEES_MILITARY: "Employees On Military Leave",
  MILITARY_FORFEITURES: "Military and Rehire Forfeitures",
  MILITARY_PROFIT_SUMMARY: "Military and Rehire Profit Summary",
  PROFIT_SHARE_REPORT: "Profit Share Report",
  PROFIT_SHARE_UPDATE: "Profit Share Update",
  MASTER_INQUIRY: "Master Inquiry",
  PROFIT_SHARE_REPORT_EDIT_RUN: "Profit Share Report Edit Run (PAY456)",
  PAY426_ACTIVE_18_20: "Active/inactive employees age 18 - 20 with 1000 hours or more",
  PAY426_ACTIVE_21_PLUS: "Active/inactive employees age 21 & with 1000 hours or more",
  PAY426_ACTIVE_UNDER_18: "Active/inactive employees under 18",
  PAY426_ACTIVE_PRIOR_SHARING: "Active/inactive employees 18 and older with prior profit sharing amounts and <1000 hours",
  PAY426_ACTIVE_NO_PRIOR: "Active/inactive employees 18 and older with no prior profit sharing amounts and <1000 hours",
  PAY426_TERMINATED_1000_PLUS: "Terminated employees 18 and older with 1000 hours or more",
  PAY426_TERMINATED_NO_PRIOR: "Terminated employees 18 and older with no prior profit sharing amounts and < 1000 hours",
  PAY426_TERMINATED_PRIOR: "Terminated employees 18 and older with prior profit sharing amounts and < 1000 hours",
  PAY426_SUMMARY: "Profit sharing summary page",
  PAY426_NON_EMPLOYEE: "All non-employee beneficiaries"
} as const;



export const VIEW = "View";