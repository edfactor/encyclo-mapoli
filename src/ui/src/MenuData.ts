import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";
import { MENU_LABELS, ROUTES, CAPTIONS } from "./constants";

const yearEndReports: RouteCategory = {
  menuLabel: MENU_LABELS.FISCAL_CLOSE,
  parentRoute: ROUTES.FISCAL_CLOSE,
  items: [
    { caption: CAPTIONS.SUMMARY, route: ROUTES.FISCAL_CLOSE },
    { caption: CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES, route: ROUTES.DISTRIBUTIONS_AND_FORFEITURES },
    { caption: CAPTIONS.MANAGE_EXECUTIVE_HOURS, route: ROUTES.MANAGE_EXECUTIVE_HOURS },
    { caption: CAPTIONS.ELIGIBLE_EMPLOYEES, route: ROUTES.ELIGIBLE_EMPLOYEES },
    { caption: CAPTIONS.DISTRIBUTIONS_BY_AGE, route: ROUTES.DISTRIBUTIONS_BY_AGE },
    { caption: CAPTIONS.CONTRIBUTIONS_BY_AGE, route: ROUTES.CONTRIBUTIONS_BY_AGE },
    { caption: CAPTIONS.FORFEITURES_BY_AGE, route: ROUTES.FORFEITURES_BY_AGE },
    { caption: CAPTIONS.BALANCE_BY_AGE, route: ROUTES.BALANCE_BY_AGE },
    { caption: CAPTIONS.BALANCE_BY_YEARS, route: ROUTES.BALANCE_BY_YEARS },
    { caption: CAPTIONS.VESTED_AMOUNTS_BY_AGE, route: ROUTES.VESTED_AMOUNTS_BY_AGE },
    { caption: CAPTIONS.TERMINATIONS, route: ROUTES.PROF_TERM },
    { caption: CAPTIONS.YTD_WAGES_EXTRACT, route: ROUTES.YTD_WAGES_EXTRACT },
    { caption: CAPTIONS.MILITARY_AND_REHIRE_ENTRY, route: ROUTES.MILITARY_AND_REHIRE_ENTRY },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN, route: ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN, route: ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN },
    { caption: CAPTIONS.PAYMASTER_UPDATE, route: ROUTES.PAYMASTER_UPDATE },
    { caption: CAPTIONS.PROFIT_SHARE_BY_STORE, route: ROUTES.PROFIT_SHARE_BY_STORE },
    { caption: CAPTIONS.PROFIT_SHARE_GROSS_REPORT, route: ROUTES.PROFIT_SHARE_GROSS_REPORT },
    { caption: CAPTIONS.FORFEIT, route: ROUTES.FORFEIT },
    { caption: CAPTIONS.PAY450_SUMMARY, route: ROUTES.PAY450_SUMMARY },
    { caption: CAPTIONS.PROFALL, route: ROUTES.PROFALL },
  ]
};

const decemberFlow: RouteCategory = {
  menuLabel: MENU_LABELS.DECEMBER_FLOW,
  parentRoute: "",
  items: [
    { caption: CAPTIONS.SUMMARY, route: ROUTES.DECEMBER_PROCESS_ACCORDION },
    { caption: CAPTIONS.DEMOGRAPHIC_BADGES, route: ROUTES.DEMOGRAPHIC_BADGES },
    { caption: CAPTIONS.DUPLICATE_SSNS, route: ROUTES.DUPLICATE_SSNS },
    { caption: CAPTIONS.NEGATIVE_ETVA, route: ROUTES.NEGATIVE_ETVA },
    { caption: CAPTIONS.DUPLICATE_NAMES, route: ROUTES.DUPLICATE_NAMES },
    { caption: CAPTIONS.MISSING_COMMA, route: ROUTES.MISSING_COMMA },
    { caption: CAPTIONS.EMPLOYEES_MILITARY, route: ROUTES.EMPLOYEES_MILITARY },
    { caption: CAPTIONS.MILITARY_FORFEITURES, route: ROUTES.MILITARY_FORFEITURES },
    { caption: CAPTIONS.MILITARY_PROFIT_SUMMARY, route: ROUTES.MILITARY_PROFIT_SUMMARY },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT, route: ROUTES.PROFIT_SHARE_REPORT },
    { caption: CAPTIONS.PROFIT_SHARE_UPDATE, route: ROUTES.PROFIT_SHARE_UPDATE }
  ]
};

const beneficiaries: RouteCategory = {
  menuLabel: MENU_LABELS.BENEFICIARIES,
  parentRoute: "",
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const distributions: RouteCategory = {
  menuLabel: MENU_LABELS.DISTRIBUTIONS,
  parentRoute: "",
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const reconciliation: RouteCategory = {
  menuLabel: MENU_LABELS.RECONCILIATION,
  parentRoute: "",
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const miscellaneous: RouteCategory = {
  menuLabel: MENU_LABELS.INQUIRIES,
  parentRoute: "",
  items: [{ caption: CAPTIONS.MASTER_INQUIRY, route: ROUTES.MASTER_INQUIRY }]
};

const MenuData: RouteCategory[] = [
  miscellaneous,
  beneficiaries,
  distributions,
  reconciliation,
  decemberFlow,
  yearEndReports
];

export default MenuData;
